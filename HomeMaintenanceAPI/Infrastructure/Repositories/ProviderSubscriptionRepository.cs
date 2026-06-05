using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class ProviderSubscriptionRepository : IProviderSubscriptionRepository
    {
        private readonly AppDbContext _context;

        public ProviderSubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProviderSubscription>> GetByProviderProfileIdAsync(int providerProfileId)
        {
            return await _context.ProviderSubscriptions
                .AsNoTracking()
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.ProviderProfileId == providerProfileId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
