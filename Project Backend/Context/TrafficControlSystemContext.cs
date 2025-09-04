using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrafficControlSystem.Entities;

namespace TrafficControlSystem.Context;


public class TrafficControlSystemContext : DbContext
{
    public TrafficControlSystemContext(DbContextOptions<TrafficControlSystemContext> optionsBuilder) : base(optionsBuilder)
    {
    }
    public DbSet<Violation> Violations { get; set; }
    public DbSet<Lane> Lanes { get; set; }
    public DbSet<Images> Images { get; set; }
    public DbSet<TrafficDensity> TrafficDensity { get; set; }
}
