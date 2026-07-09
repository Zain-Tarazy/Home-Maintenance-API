using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface IProviderOfferRepository
    {
        Task<ProviderOffer?> GetByIdWithDetailsAsync(int id);

        Task<List<ProviderOffer>> GetByOrderIdAsync(int orderId);

        Task<PagedResult<ProviderOffer>> GetByProviderProfileIdAsync(int providerProfileId,PaginationParams paginationParams);

        Task<bool> ExistsForOrderAndProviderAsync(int orderId, int providerProfileId);

        Task<bool> HasActiveAcceptedOfferAsync(int orderId);

        Task<List<ProviderOffer>> GetPendingOffersForOrderExceptAsync(int orderId, int excludedOfferId);

        Task<ProviderOffer> AddAsync(ProviderOffer offer);

        Task UpdateAsync(ProviderOffer offer);

        Task UpdateRangeAsync(List<ProviderOffer> offers);

        Task SaveChangesAsync();
    }
}
