using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _context;

        public RatingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsForOrderAsync(int orderId)
        {
            return await _context.Ratings
                .AnyAsync(r => r.OrderId == orderId);
        }

        public async Task<Rating?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Ratings
                .Include(r => r.Customer)
                .Include(r => r.ProviderProfile)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Rating>> GetByProviderProfileIdAsync(int providerProfileId)
        {
            return await _context.Ratings
                .AsNoTracking()
                .Where(r => r.ProviderProfileId == providerProfileId)
                .ToListAsync();
        }

        public async Task<int> GetCompletedOrdersCountAsync(int providerProfileId)
        {
            return await _context.Orders
                .CountAsync(o =>
                    o.SelectedProviderProfileId == providerProfileId &&
                    o.Status == OrderStatus.Completed);
        }

        public async Task<Rating> AddAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            return rating;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
