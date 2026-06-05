using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);

        Task<Order?> GetByIdWithDetailsAsync(int id);

        Task<List<Order>> GetByCustomerIdAsync(int customerId);

        Task<List<Order>> GetAvailableForProviderAsync(int specializationId, int providerUserId);

        Task<List<Order>> GetAssignedForProviderAsync(int providerProfileId);

        Task<bool> HasOffersAsync(int orderId);

        Task<Order> AddAsync(Order order);

        Task UpdateAsync(Order order);

        Task SaveChangesAsync();
    }
}
