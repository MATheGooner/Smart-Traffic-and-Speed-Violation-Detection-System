using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Interface.Services;

public interface ITrafficDensityService
{
    public Task<BaseResponse> CreateTrafficDensity(CreateTrafficDensityDto createTrafficDensityDto);
    public Task<TrafficDensitysResponse> GetAllTrafficDensitys();
}
