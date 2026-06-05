using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IProviderProfileService
    {
        Task<ServiceResult<ProviderProfile>> CreateAsync(int userId, CreateProviderProfileDto dto);

        Task<ServiceResult<ProviderProfile>> GetMineAsync(int userId);

        Task<ServiceResult<ProviderProfile>> UpdateMineAsync(int userId, UpdateProviderProfileDto dto);

        Task<ServiceResult<ProviderSubscriptionStatusDto>> GetSubscriptionStatusAsync(int userId);
    }
}

