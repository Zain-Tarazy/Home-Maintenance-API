using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Specialization;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface ISpecializationService
    {
        Task<List<Specialization>> GetActiveAsync();

        Task<List<Specialization>> GetAllAsync();

        Task<ServiceResult<Specialization>> CreateAsync(CreateSpecializationDto dto);

        Task<ServiceResult<Specialization>> UpdateAsync(int id, UpdateSpecializationDto dto);

        Task<ServiceResult> ActivateAsync(int id);

        Task<ServiceResult> DeactivateAsync(int id);
    }
}
