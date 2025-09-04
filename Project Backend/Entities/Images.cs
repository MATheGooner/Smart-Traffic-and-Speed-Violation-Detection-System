using TrafficControlSystem.Contracts;

namespace TrafficControlSystem.Entities;

public class Images : AuditableEntity
{
    public int Id { get; set; }
    public int LaneId { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

}