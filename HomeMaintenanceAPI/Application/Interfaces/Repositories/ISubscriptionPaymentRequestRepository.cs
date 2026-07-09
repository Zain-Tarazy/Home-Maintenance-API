using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface ISubscriptionPaymentRequestRepository
    {
        Task<List<SubscriptionPaymentRequest>> GetByProviderProfileIdAsync(int providerProfileId);

        Task<PagedResult<SubscriptionPaymentRequest>> GetAllAsync(PaginationParams paginationParams);

        Task<PagedResult<SubscriptionPaymentRequest>> GetPendingAsync(PaginationParams paginationParams);

        Task<SubscriptionPaymentRequest?> GetByIdAsync(int id);

        Task<bool> TransactionIdExistsAsync(string transactionId);

        Task<bool> HasPendingRequestAsync(int providerProfileId);

        Task<SubscriptionPaymentRequest> AddAsync(SubscriptionPaymentRequest request);

        Task AddSubscriptionAsync(ProviderSubscription subscription);

        Task UpdateAsync(SubscriptionPaymentRequest request);

        Task SaveChangesAsync();
    }
}