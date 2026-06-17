using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface ISubscriptionPaymentRequestRepository
    {
        Task<List<SubscriptionPaymentRequest>> GetByProviderProfileIdAsync(int providerProfileId);

        Task<List<SubscriptionPaymentRequest>> GetAllAsync();

        Task<List<SubscriptionPaymentRequest>> GetPendingAsync();

        Task<SubscriptionPaymentRequest?> GetByIdAsync(int id);

        Task<bool> TransactionIdExistsAsync(string transactionId);

        Task<bool> HasPendingRequestAsync(int providerProfileId);

        Task<SubscriptionPaymentRequest> AddAsync(SubscriptionPaymentRequest request);

        Task AddSubscriptionAsync(ProviderSubscription subscription);

        Task UpdateAsync(SubscriptionPaymentRequest request);

        Task SaveChangesAsync();
    }
}