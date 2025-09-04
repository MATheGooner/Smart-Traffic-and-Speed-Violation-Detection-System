using TrafficControlSystem.Models.Emums;

namespace TrafficControlSystem.Models.DTOs;

public class CreateLaneDto
{
    public string Name { get; set; }
    public int MaxSpeed { get; set; }
    public LaneDensity Density { get; set; }
}
public class UpdateLaneDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MaxSpeed { get; set; }
    public LaneDensity Density { get; set; }
}
public class GetLaneDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MaxSpeed { get; set; }
    public LaneDensity Density { get; set; }
}
public class LanesResponse : BaseResponse
{
    public ICollection<GetLaneDto> Lanes { get; set; } = new HashSet<GetLaneDto>();
}
