using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface ISubscriptionPlanRepository
    {
        Task<List<SubscriptionPlan>> GetActiveAsync();

        Task<List<SubscriptionPlan>> GetAllAsync();

        Task<SubscriptionPlan?> GetByIdAsync(int id);

        Task<bool> NameExistsAsync(string name);

        Task<bool> NameExistsExceptAsync(string name, int id);

        Task<bool> IsUsedAsync(int id);

        Task<SubscriptionPlan> AddAsync(SubscriptionPlan plan);

        Task UpdateAsync(SubscriptionPlan plan);

        Task SaveChangesAsync();
    }
}
