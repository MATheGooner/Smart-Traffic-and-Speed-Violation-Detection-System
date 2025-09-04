using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Interface.Services;

public interface IImageService
{
    public Task<BaseResponse> CreateImage(CreateImageDto createImageDto);
    public Task<ImagesResponse> GetRecentImages();
}