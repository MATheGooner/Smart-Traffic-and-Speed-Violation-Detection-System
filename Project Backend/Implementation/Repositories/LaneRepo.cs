using TrafficControlSystem.Context;
using TrafficControlSystem.Entities;
using TrafficControlSystem.Interface.Respositories;

namespace TrafficControlSystem.Implementation.Respositories;
public class LaneRepo : BaseRepository<Lane>, ILaneRepo
{

    public LaneRepo(TrafficControlSystemContext _context)
    {
        context = _context;
    }

}