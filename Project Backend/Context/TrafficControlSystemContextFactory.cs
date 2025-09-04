using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TrafficControlSystem.Context
{
    public class TrafficControlSystemContextFactory : IDesignTimeDbContextFactory<TrafficControlSystemContext>
    {
        public TrafficControlSystemContext CreateDbContext(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();
            var connectionString = config.GetConnectionString("TrafficControlSystemContext");

            var optionsBuilder = new DbContextOptionsBuilder<TrafficControlSystemContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new TrafficControlSystemContext(optionsBuilder.Options);
        }
    }
}
