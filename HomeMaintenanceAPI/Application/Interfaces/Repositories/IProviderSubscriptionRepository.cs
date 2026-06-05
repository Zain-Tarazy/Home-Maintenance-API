using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface IProviderSubscriptionRepository
    {
        Task<List<ProviderSubscription>> GetByProviderProfileIdAsync(int providerProfileId);
    }
}
