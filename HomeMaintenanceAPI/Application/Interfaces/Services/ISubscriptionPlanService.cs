using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface ISubscriptionPlanService
    {
        Task<List<SubscriptionPlan>> GetActiveAsync();

        Task<List<SubscriptionPlan>> GetAllAsync();

        Task<ServiceResult<SubscriptionPlan>> CreateAsync(CreateSubscriptionPlanDto dto);

        Task<ServiceResult<SubscriptionPlan>> UpdateAsync(int id, UpdateSubscriptionPlanDto dto);

        Task<ServiceResult> ActivateAsync(int id);

        Task<ServiceResult> DeactivateAsync(int id);
    }
}
