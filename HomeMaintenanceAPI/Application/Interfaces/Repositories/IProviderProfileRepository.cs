using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface IProviderProfileRepository
    {
        Task<ProviderProfile?> GetByUserIdAsync(int userId);

        Task<ProviderProfile?> GetByIdAsync(int id);

        Task<bool> ExistsForUserAsync(int userId);

        Task<ProviderProfile> AddAsync(ProviderProfile providerProfile);

        Task UpdateAsync(ProviderProfile providerProfile);

        Task<bool> HasActiveSubscriptionAsync(int providerProfileId);

        Task<DateTime?> GetActiveSubscriptionEndDateAsync(int providerProfileId);

        Task SaveChangesAsync();
    }
}
