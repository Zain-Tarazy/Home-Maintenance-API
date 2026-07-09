using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Offers;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IProviderOfferService
    {
        Task<ServiceResult<ProviderOffer>> CreateAsync(int userId, CreateOfferDto dto);

        Task<ServiceResult<PagedResult<ProviderOffer>>> GetMineAsync(int providerUserId, PaginationParams paginationParams);

        Task<ServiceResult<List<ProviderOffer>>> GetByOrderIdAsync(int userId, int orderId);

        Task<ServiceResult<ProviderOffer>> UpdateAsync(int userId, int offerId, UpdateOfferDto dto);

        Task<ServiceResult> CancelAsync(int userId, int offerId);

        Task<ServiceResult> RejectAsync(int userId, int offerId);

        Task<ServiceResult<ProviderOffer>> AcceptForInspectionAsync(int userId, int offerId);

        Task<ServiceResult<ProviderOffer>> RejectAfterInspectionAsync(int userId, int offerId);

        Task<ServiceResult<ProviderOffer>> ContinueWorkAsync(int userId, int offerId);
    }
}
