using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrafficControlSystem.Implementation.Services;
using TrafficControlSystem.Interface.Services;
using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Controllers;

[Route("TrafficControlSystem/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    IImageService _imagesService;
    public ImagesController(IImageService imagesService)
    {
        _imagesService = imagesService;
    }
    // POST : AddImages
    [HttpPost("CreateImages")]
    public async Task<IActionResult> CreateImages([FromForm] CreateImageDto createImagesDto)
    {
        var images = await _imagesService.CreateImage(createImagesDto);
        if (images.Status == true)
        {
            return Ok(images);
        }
        return BadRequest(images);
    }
    // GET : GetRecentImages
    [HttpGet("GetRecentImages")]
    public async Task<IActionResult> GetRecentImages()
    {
        var imagess = await _imagesService.GetRecentImages();
        if (imagess.Status == true)
        {
            return Ok(imagess);
        }
        return BadRequest(imagess);
    }
}