using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class ProviderProfileRepository : IProviderProfileRepository
    {
        private readonly AppDbContext _context;

        public ProviderProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderProfile?> GetByUserIdAsync(int userId)
        {
            return await _context.ProviderProfiles
                .Include(p => p.User)
                .Include(p => p.Specialization)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<ProviderProfile?> GetByIdAsync(int id)
        {
            return await _context.ProviderProfiles
                .Include(p => p.User)
                .Include(p => p.Specialization)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsForUserAsync(int userId)
        {
            return await _context.ProviderProfiles
                .AnyAsync(p => p.UserId == userId);
        }

        public async Task<ProviderProfile> AddAsync(ProviderProfile providerProfile)
        {
            await _context.ProviderProfiles.AddAsync(providerProfile);
            return providerProfile;
        }

        public Task UpdateAsync(ProviderProfile providerProfile)
        {
            _context.ProviderProfiles.Update(providerProfile);
            return Task.CompletedTask;
        }

        public async Task<bool> HasActiveSubscriptionAsync(int providerProfileId)
        {
            var now = DateTime.UtcNow;

            return await _context.ProviderSubscriptions
                .AnyAsync(s =>
                    s.ProviderProfileId == providerProfileId &&
                    s.StartsAt <= now &&
                    s.EndsAt > now);
        }

        public async Task<DateTime?> GetActiveSubscriptionEndDateAsync(int providerProfileId)
        {
            var now = DateTime.UtcNow;

            return await _context.ProviderSubscriptions
                .Where(s =>
                    s.ProviderProfileId == providerProfileId &&
                    s.StartsAt <= now &&
                    s.EndsAt > now)
                .OrderByDescending(s => s.EndsAt)
                .Select(s => (DateTime?)s.EndsAt)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
