using TrafficControlSystem.Entities;
using TrafficControlSystem.Interface.Respositories;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Implementation.Services;
public class LaneService : ILaneService
{
    private readonly ILaneRepo _laneRepo;

    public LaneService(ILaneRepo laneRepo)
    {
        _laneRepo = laneRepo;
    }

    public async Task<BaseResponse> CreateLane(CreateLaneDto createLaneDto)
    {
        if (createLaneDto != null)
        {
            var lane = new Lane()
            {
                Name = createLaneDto.Name,
                MaxSpeed = createLaneDto.MaxSpeed,
                Density = createLaneDto.Density,
            };
            await _laneRepo.Create(lane);
            return new BaseResponse
            {
                Status = true,
                Message = "Lane created successfully."
            };
        }
        return new BaseResponse
        {
            Status = false,
            Message = "Invalid lane data."
        };
    }
    public async Task<BaseResponse> UpdateLane(UpdateLaneDto updateLaneDto)
    {
        if (updateLaneDto != null)
        {
            var lane = (await _laneRepo.GetByExpression(x => x.Id == updateLaneDto.Id)).FirstOrDefault();
            if (lane != null)
            {
                lane.Name = updateLaneDto.Name;
                lane.MaxSpeed = updateLaneDto.MaxSpeed;
                lane.Density = updateLaneDto.Density;
                await _laneRepo.Update(lane);
                return new BaseResponse
                {
                    Status = true,
                    Message = "Lane updated successfully."
                };
            }
            return new BaseResponse
            {
                Status = false,
                Message = "Lane not found."
            };
        }
        return new BaseResponse
        {
            Status = false,
            Message = "Invalid lane data."
        };
    }
    public async Task<LanesResponse> GetAllLanes()
    {
        var lanes = await _laneRepo.GetAll();
        if (lanes != null && lanes.Any())
        {
            return new LanesResponse
            {
                Status = true,
                Message = "Lanes retrieved successfully.",
                Lanes = lanes.Select(x => new GetLaneDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    MaxSpeed = x.MaxSpeed,
                    Density = x.Density
                }).ToList()
            };
        }
        
        return new LanesResponse
        {
            Status = false,
            Message = "No lanes found."
        };
    }
}