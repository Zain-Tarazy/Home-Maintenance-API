using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IProviderSubscriptionService
    {
        Task<ServiceResult<List<ProviderSubscription>>> GetMineAsync(int userId);
    }
}
