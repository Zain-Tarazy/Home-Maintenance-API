using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface IRatingRepository
    {
        Task<bool> ExistsForOrderAsync(int orderId);

        Task<Rating?> GetByIdWithDetailsAsync(int id);

        Task<List<Rating>> GetByProviderProfileIdAsync(int providerProfileId);

        Task<int> GetCompletedOrdersCountAsync(int providerProfileId);

        Task<Rating> AddAsync(Rating rating);

        Task SaveChangesAsync();
    }
}
