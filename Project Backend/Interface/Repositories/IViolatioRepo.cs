using TrafficControlSystem.Entities;

namespace TrafficControlSystem.Interface.Respositories;

public interface IViolationRepo : IBaseRepo<Violation>
{
    public Task<List<Violation>> GetAllViolations();
}