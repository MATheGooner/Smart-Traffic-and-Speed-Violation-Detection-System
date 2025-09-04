namespace TrafficControlSystem.Models.DTOs;

public class CreateImageDto
{
    public int LaneId { get; set; }
    public IFormFile ImageFile { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class GetImageDto
{
    public int Id { get; set; }
    public int LaneId { get; set; }
    public string ImageBlob { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class ImagesResponse : BaseResponse
{
    public ICollection<GetImageDto> Images { get; set; } = new HashSet<GetImageDto>();
}