using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Services
{
    public class ProviderSubscriptionService : IProviderSubscriptionService
    {
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly IProviderSubscriptionRepository _providerSubscriptionRepository;

        public ProviderSubscriptionService(
            IProviderProfileRepository providerProfileRepository,
            IProviderSubscriptionRepository providerSubscriptionRepository)
        {
            _providerProfileRepository = providerProfileRepository;
            _providerSubscriptionRepository = providerSubscriptionRepository;
        }

        public async Task<ServiceResult<List<ProviderSubscription>>> GetMineAsync(int userId)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult<List<ProviderSubscription>>.Failure("Provider profile not found.");

            var subscriptions =
                await _providerSubscriptionRepository.GetByProviderProfileIdAsync(providerProfile.Id);

            return ServiceResult<List<ProviderSubscription>>.Success(subscriptions);
        }
    }
}
