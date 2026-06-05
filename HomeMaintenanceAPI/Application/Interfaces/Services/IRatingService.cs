using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Ratings;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IRatingService
    {
        Task<ServiceResult<Rating>> CreateAsync(int userId, CreateRatingDto dto);

        Task<ServiceResult<ProviderRatingSummaryDto>> GetProviderSummaryAsync(int providerProfileId);
    }
}
