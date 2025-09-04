using TrafficControlSystem.Contracts;

namespace TrafficControlSystem.Entities;

public class Violation : AuditableEntity
{
    public int Id { get; set; }
    public int LaneId { get; set; }
    public Lane Lane { get; set; }
    public int Speed { get; set; }
    public string ImageUrl { get; set; }
    public DateTime ViolationTime { get; set; }
}