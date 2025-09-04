using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrafficControlSystem.Implementation.Services;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Controllers;

[Route("TrafficControlSystem/[controller]")]
[ApiController]
public class ViolationController : ControllerBase
{
    IViolationService _violationService;
    public ViolationController(IViolationService violationService)
    {
        _violationService = violationService;
    }
    // POST : AddViolation
    [HttpPost("CreateViolation")]
    public async Task<IActionResult> CreateViolation(CreateViolationDto createViolationDto)
    {
        var violation = await _violationService.CreateViolation(createViolationDto);
        if (violation.Status == true)
        {
            return Ok(violation);
        }
        return BadRequest(violation);
    }
    // GET : GetAllViolations
    [HttpGet("GetAllViolations")]
    public async Task<IActionResult> GetAllViolations()
    {
        var violations = await _violationService.GetAllViolations();
        if (violations.Status == true)
        {
            return Ok(violations);
        }
        return BadRequest(violations);
    }
}