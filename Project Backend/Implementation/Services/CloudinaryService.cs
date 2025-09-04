using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using TrafficControlSystem.Interface.Services;
namespace TrafficControlSystem.Implementation.Services;
public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService()
    {
        Account account = new Account(
            "dj8cfcq4x",
            "992598965352432",
            "NItNFiKswHqavZTlyTS-hgDjgcY");

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(string filePath, MemoryStream stream)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(filePath, stream),
            Folder = "uploads", // Optional: save to a specific folder
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return uploadResult.SecureUrl.ToString(); // Get the image URL
        }
        else
        {
            throw new Exception("Image upload failed: " + uploadResult.Error?.Message);
        }
    }
}
