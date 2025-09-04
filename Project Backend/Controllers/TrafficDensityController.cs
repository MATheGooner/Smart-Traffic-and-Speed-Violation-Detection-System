using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrafficControlSystem.Implementation.Services;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Controllers;

[Route("TrafficControlSystem/[controller]")]
[ApiController]
public class TrafficDensityController : ControllerBase
{
    ITrafficDensityService _trafficDensityService;
    public TrafficDensityController(ITrafficDensityService trafficDensityService)
    {
        _trafficDensityService = trafficDensityService;
    }
    // POST : AddTrafficDensity
    [HttpPost("CreateTrafficDensity")]
    public async Task<IActionResult> CreateTrafficDensity(CreateTrafficDensityDto createTrafficDensityDto)
    {
        var trafficDensity = await _trafficDensityService.CreateTrafficDensity(createTrafficDensityDto);
        if (trafficDensity.Status == true)
        {
            return Ok(trafficDensity);
        }
        return Ok(trafficDensity);
    }
    // GET : GetAllTrafficDensitys
    [HttpGet("GetAllTrafficDensitys")]
    public async Task<IActionResult> GetAllTrafficDensitys()
    {
        var trafficDensitys = await _trafficDensityService.GetAllTrafficDensitys();
        if (trafficDensitys.Status == true)
        {
            return Ok(trafficDensitys);
        }
        return Ok(trafficDensitys);
    }
}