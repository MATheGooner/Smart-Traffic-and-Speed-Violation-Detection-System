using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Interface.Services;

public interface ILaneService
{
    public Task<BaseResponse> CreateLane(CreateLaneDto createLaneDto);
    public Task<BaseResponse> UpdateLane(UpdateLaneDto updateLaneDto);
    public Task<LanesResponse> GetAllLanes();
}
