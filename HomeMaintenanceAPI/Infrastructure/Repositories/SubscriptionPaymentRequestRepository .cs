using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class SubscriptionPaymentRequestRepository : ISubscriptionPaymentRequestRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionPaymentRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionPaymentRequest>> GetByProviderProfileIdAsync(int providerProfileId)
        {
            return await _context.SubscriptionPaymentRequests
                .AsNoTracking()
                .Include(r => r.ProviderProfile)
                    .ThenInclude(p => p.User)
                .Include(r => r.SubscriptionPlan)
                .Include(r => r.ReviewedByAdmin)
                .Where(r => r.ProviderProfileId == providerProfileId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResult<SubscriptionPaymentRequest>> GetPendingAsync(PaginationParams paginationParams)
        {
            var query = _context.SubscriptionPaymentRequests
                .AsNoTracking()
                .Include(r => r.ProviderProfile)
                    .ThenInclude(p => p.User)
                .Include(r => r.SubscriptionPlan)
                .Include(r => r.ReviewedByAdmin)
                .Where(r => r.Status == SubscriptionPaymentRequestStatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);
        }
        public async Task<PagedResult<SubscriptionPaymentRequest>> GetAllAsync(PaginationParams paginationParams)
        {
            var query = _context.SubscriptionPaymentRequests
                .AsNoTracking()
                .Include(r => r.ProviderProfile)
                    .ThenInclude(p => p.User)
                .Include(r => r.SubscriptionPlan)
                .Include(r => r.ReviewedByAdmin)
                .OrderByDescending(r => r.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);
        }

        public async Task<SubscriptionPaymentRequest?> GetByIdAsync(int id)
        {
            return await _context.SubscriptionPaymentRequests
                .Include(r => r.ProviderProfile)
                    .ThenInclude(p => p.User)
                .Include(r => r.SubscriptionPlan)
                .Include(r => r.ReviewedByAdmin)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> TransactionIdExistsAsync(string transactionId)
        {
            return await _context.SubscriptionPaymentRequests
                .AnyAsync(r => r.TransactionId == transactionId);
        }

        public async Task<bool> HasPendingRequestAsync(int providerProfileId)
        {
            return await _context.SubscriptionPaymentRequests
                .AnyAsync(r =>
                    r.ProviderProfileId == providerProfileId &&
                    r.Status == SubscriptionPaymentRequestStatus.Pending);
        }

        public async Task<SubscriptionPaymentRequest> AddAsync(SubscriptionPaymentRequest request)
        {
            await _context.SubscriptionPaymentRequests.AddAsync(request);
            return request;
        }

        public async Task AddSubscriptionAsync(ProviderSubscription subscription)
        {
            await _context.ProviderSubscriptions.AddAsync(subscription);
        }

        public Task UpdateAsync(SubscriptionPaymentRequest request)
        {
            _context.SubscriptionPaymentRequests.Update(request);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
