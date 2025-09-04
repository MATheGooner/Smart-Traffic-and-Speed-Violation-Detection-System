using TrafficControlSystem.Contracts;
using TrafficControlSystem.Models.Emums;

namespace TrafficControlSystem.Entities;

public class Lane : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MaxSpeed { get; set; }
    public LaneDensity Density { get; set; }
}