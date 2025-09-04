using TrafficControlSystem.Entities;
using TrafficControlSystem.Interface.Respositories;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Implementation.Services;

public class ImageService : IImageService
{
    private readonly IImageRepo _imageRepo;
    private readonly ILaneRepo _laneRepo;

    public ImageService(IImageRepo imageRepo, ILaneRepo laneRepo)
    {
        _laneRepo = laneRepo;
        _imageRepo = imageRepo;
    }

    public async Task<BaseResponse> CreateImage(CreateImageDto createImageDto)
    {
        if (createImageDto != null)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory() + "..\\Images\\");
            if (!System.IO.Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var imagePath = "";
            if (createImageDto.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString().Substring(0, 15);
                var filePath = Path.Combine(folderPath, fileName);
                var extension = Path.GetExtension(createImageDto.ImageFile.FileName);
                if (!System.IO.Directory.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createImageDto.ImageFile.CopyToAsync(stream);
                    }
                    imagePath = fileName;
                }
            }
            var image = new Images()
            {
                LaneId = createImageDto.LaneId,
                ImageUrl = imagePath,
            };
            await _imageRepo.Create(image);
            return new BaseResponse
            {
                Status = true,
                Message = "Image uploaded successfully."
            };
        }
        return new BaseResponse
        {
            Status = false,
            Message = "Invalid image data."
        };
    }

    public async Task<ImagesResponse> GetRecentImages()
    {
        var laneIds = new List<int>();
        var lanes = await _laneRepo.GetAll();
        if (lanes != null) {
            foreach (var lane in lanes)    laneIds.Add(lane.Id);
        }
        var images = await _imageRepo.GetAll();
        var ImagesList = new List<Images>();
        if (images != null && images.Any())
        {
            foreach(var id in laneIds)  ImagesList.Add(images.LastOrDefault(x => x.LaneId == id));
            return new ImagesResponse
            {
                Status = true,
                Message = "Images retrieved successfully.",
                Images = ImagesList.Select(x => new GetImageDto
                {
                    Id = x.Id,
                    LaneId = x.LaneId,
                    ImageBlob = GetImageBlob(x.ImageUrl),
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
        return new ImagesResponse
        {
            Status = false,
            Message = "No images found."
        };
    }
    public string GetImageBlob(string imageUrl)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory() + "..\\Images\\");
        var filePath = Path.Combine(folderPath, imageUrl);
        if (File.Exists(filePath))
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(imageBytes);
        }
        return null;
    }
}