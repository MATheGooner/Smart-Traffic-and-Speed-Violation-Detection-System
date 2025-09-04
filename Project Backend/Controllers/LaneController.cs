using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrafficControlSystem.Implementation.Services;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Controllers;

[Route("TrafficControlSystem/[controller]")]
[ApiController]
public class LaneController : ControllerBase
{
    ILaneService _laneService;
    public LaneController(ILaneService laneService)
    {
        _laneService = laneService;
    }
    // POST : AddLane
    [HttpPost("CreateLane")]
    public async Task<IActionResult> CreateLane(CreateLaneDto createLaneDto)
    {
        var lane = await _laneService.CreateLane(createLaneDto);
        if (lane.Status == true)
        {
            return Ok(lane);
        }
        return BadRequest(lane);
    }

    // PUT : UpdateLane
    [HttpPut("UpdateLane")]
    public async Task<IActionResult> UpdateLane(UpdateLaneDto updateLaneDto)
    {
        var lane = await _laneService.UpdateLane(updateLaneDto);
        if (lane.Status == true)
        {
            return Ok(lane);
        }
        return BadRequest(lane);
    }
    // GET : GetAllLanes
    [HttpGet("GetAllLanes")]
    public async Task<IActionResult> GetAllLanes()
    {
        var lanes = await _laneService.GetAllLanes();
        if (lanes.Status == true)
        {
            return Ok(lanes);
        }
        return BadRequest(lanes);
    }
}