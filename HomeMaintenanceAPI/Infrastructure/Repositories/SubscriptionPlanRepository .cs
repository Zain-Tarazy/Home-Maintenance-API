using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionPlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionPlan>> GetActiveAsync()
        {
            return await _context.SubscriptionPlans
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await _context.SubscriptionPlans
                .AsNoTracking()
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.SubscriptionPlans
                .AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> NameExistsExceptAsync(string name, int id)
        {
            return await _context.SubscriptionPlans
                .AnyAsync(p => p.Id != id && p.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> IsUsedAsync(int id)
        {
            return await _context.SubscriptionPaymentRequests
                       .AnyAsync(r => r.SubscriptionPlanId == id)
                   || await _context.ProviderSubscriptions
                       .AnyAsync(s => s.SubscriptionPlanId == id);
        }

        public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan plan)
        {
            await _context.SubscriptionPlans.AddAsync(plan);
            return plan;
        }

        public Task UpdateAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
