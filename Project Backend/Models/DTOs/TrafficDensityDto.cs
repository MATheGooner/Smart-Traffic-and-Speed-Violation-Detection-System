using TrafficControlSystem.Models.Emums;

namespace TrafficControlSystem.Models.DTOs;

public class CreateTrafficDensityDto
{
    public int Id { get; set; }
    public int LaneId { get; set; }
    public int Density { get; set; }
    public string traffic_status { get; set; }
    public int density_percentage { get; set; }
    public DateTime Date { get; set; }
}
public class GetTrafficDensityDto
{
    public int Id { get; set; }
    public int LaneId { get; set; }
    public int Density { get; set; }
    public DateTime Date { get; set; }
    public string lane_id { get; set; }
    public string traffic_status { get; set; }
    public int density_percentage { get; set; }
}
public class TrafficDensitysResponse : BaseResponse
{
    public ICollection<GetTrafficDensityDto> TrafficDensitys { get; set; } = new HashSet<GetTrafficDensityDto>();
}
