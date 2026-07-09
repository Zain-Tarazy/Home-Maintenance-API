using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);

        Task<Order?> GetByIdWithDetailsAsync(int id);

        Task<PagedResult<Order>> GetByCustomerIdAsync(int customerId, PaginationParams paginationParams);

        Task<PagedResult<Order>> GetAvailableForProviderAsync(int specializationId, int providerUserId, PaginationParams paginationParams);

        Task<PagedResult<Order>> GetAssignedForProviderAsync(int providerProfileId, PaginationParams paginationParams);

        Task<bool> HasOffersAsync(int orderId);

        Task<Order> AddAsync(Order order);

        Task UpdateAsync(Order order);

        Task SaveChangesAsync();
    }
}
