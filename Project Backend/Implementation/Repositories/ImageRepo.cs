using TrafficControlSystem.Context;
using TrafficControlSystem.Entities;
using TrafficControlSystem.Interface.Respositories;

namespace TrafficControlSystem.Implementation.Respositories;
public class ImageRepo : BaseRepository<Images>, IImageRepo
{
    public ImageRepo(TrafficControlSystemContext _context)
    {
        context = _context;
    }

}