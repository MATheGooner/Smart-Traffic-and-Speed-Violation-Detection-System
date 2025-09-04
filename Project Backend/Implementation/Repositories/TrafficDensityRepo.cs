using TrafficControlSystem.Context;
using TrafficControlSystem.Entities;
using TrafficControlSystem.Interface.Respositories;

namespace TrafficControlSystem.Implementation.Respositories;
public class TrafficDensityRepo : BaseRepository<TrafficDensity>, ITrafficDensityRepo
{
    public TrafficDensityRepo(TrafficControlSystemContext _context)
    {
        context = _context;
    }

}