using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<int> CountUsersAsync();

        Task<int> CountProvidersAsync();

        Task<int> CountOrdersAsync();

        Task<int> CountOrdersByStatusAsync(OrderStatus status);

        Task<int> CountPendingSubscriptionRequestsAsync();

        Task<int> CountActiveSubscriptionsAsync();

        Task<List<User>> GetUsersAsync();

        Task<List<ProviderProfile>> GetProvidersAsync();

        Task<List<Order>> GetOrdersAsync();
    }
}