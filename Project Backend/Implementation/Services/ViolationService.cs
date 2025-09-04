using TrafficControlSystem.Entities;
using TrafficControlSystem.Interface.Respositories;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Implementation.Services;

public class ViolationService : IViolationService
{
    IViolationRepo _violationRepo;
    ICloudinaryService _cloudinaryService;
    public ViolationService(IViolationRepo violationRepo, ICloudinaryService cloudinaryService)
    {
        _violationRepo = violationRepo;
        _cloudinaryService = cloudinaryService;
    }
    public async Task<BaseResponse> CreateViolation(CreateViolationDto createViolationDto)
    {
        if (createViolationDto.Image != null)
        {
            if (createViolationDto.Image.Contains(","))
            createViolationDto.Image = createViolationDto.Image.Substring(createViolationDto.Image.IndexOf(",") + 1);

            byte[] imageBytes;

            try
            {
                imageBytes = Convert.FromBase64String(createViolationDto.Image);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid base64 string.");
            }

            using var stream = new MemoryStream(imageBytes);

            var cloudinaryResponse = await _cloudinaryService.UploadImageAsync($"{createViolationDto.LaneId}-{createViolationDto.Speed}.jpg", stream);

            Console.WriteLine("Cloudinary Response: " + cloudinaryResponse);
            var violation = new Violation()
            {
                LaneId = createViolationDto.LaneId,
                ViolationTime = DateTime.UtcNow,
                Speed = createViolationDto.Speed,
                ImageUrl = cloudinaryResponse, 
            };
            await _violationRepo.Create(violation);
            return new BaseResponse
            {
                Status = true,
                Message = "Violation created successfully."
            };
        }
        return new BaseResponse
        {
            Status = false,
            Message = "Invalid violation data."
        };
    }
    public async Task<ViolationsResponse> GetAllViolations()
    {
        var violations = await _violationRepo.GetAllViolations();
        if (violations != null && violations.Any())
        {
            return new ViolationsResponse
            {
                Status = true,
                Message = "Violations retrieved successfully.",
                Violations = violations.OrderByDescending(x => x.ViolationTime).Select(x => new GetViolationDto
                {
                    Id = x.Id,
                    LaneId = x.LaneId,
                    Speed = x.Speed,
                    ViolationTime = x.ViolationTime,
                    LaneName = x.Lane.Name,
                    SpeedLimit = x.Lane.MaxSpeed,
                    ViolationDateString = x.ViolationTime.Date.ToString("yyyy-MM-dd"),
                    ViolationTimeString = x.ViolationTime.TimeOfDay.ToString(),
                    ImageUrl = x.ImageUrl
                }).ToList()
            };
        }
        return new ViolationsResponse
        {
            Status = false,
            Message = "No violations found."
        };
    }
}

