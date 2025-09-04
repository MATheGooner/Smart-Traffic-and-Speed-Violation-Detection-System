using TrafficControlSystem.Models.DTOs;

namespace TrafficControlSystem.Interface.Services;

public interface IViolationService
{
    public Task<BaseResponse> CreateViolation(CreateViolationDto createViolationDto);
    public Task<ViolationsResponse> GetAllViolations();
}
