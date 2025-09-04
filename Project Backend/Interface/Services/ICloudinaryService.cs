namespace TrafficControlSystem.Interface.Services;
public interface ICloudinaryService
{
    Task<string> UploadImageAsync(string filePath, MemoryStream stream);
}

