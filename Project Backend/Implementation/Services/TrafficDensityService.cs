using TrafficControlSystem.Entities;
using TrafficControlSystem.Interface.Respositories;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Implementation.Services;
public class TrafficDensityService : ITrafficDensityService
{
    private readonly ITrafficDensityRepo _trafficDensityRepo;

    public TrafficDensityService(ITrafficDensityRepo trafficDensityRepo)
    {
        _trafficDensityRepo = trafficDensityRepo;
    }

    public async Task<BaseResponse> CreateTrafficDensity(CreateTrafficDensityDto createTrafficDensityDto)
    {
        if (createTrafficDensityDto != null)
        {
            var trafficDensity = new TrafficDensity()
            {
                LaneId = createTrafficDensityDto.LaneId,
                Density = createTrafficDensityDto.Density,
density_percentage = 0,
traffic_status = "",
                Date = createTrafficDensityDto.Date
            };
            await _trafficDensityRepo.Create(trafficDensity);
            return new BaseResponse
            {
                Status = true,
                Message = "TrafficDensity created successfully."
            };
        }
        return new BaseResponse
        {
            Status = false,
            Message = "Invalid trafficDensity data."
        };
    }
    public async Task<TrafficDensitysResponse> GetAllTrafficDensitys()
    {
        var trafficDensitys = await _trafficDensityRepo.GetAll();
        if (trafficDensitys != null && trafficDensitys.Any())
        {
            return new TrafficDensitysResponse
            {
                Status = true,
                Message = "TrafficDensitys retrieved successfully.",
                TrafficDensitys = trafficDensitys.Select(x => new GetTrafficDensityDto
                {
                    Id = x.Id,
                    LaneId = x.LaneId,
                    Density = x.Density,
                    Date = x.Date,
                    traffic_status = x.traffic_status,
                    density_percentage = x.density_percentage,
                    
                }).OrderBy(x => x.LaneId).ThenBy(x => x.Date).ToList()
            };
        }
        
        return new TrafficDensitysResponse
        {
            Status = false,
            Message = "No trafficDensitys found."
        };
    }
}