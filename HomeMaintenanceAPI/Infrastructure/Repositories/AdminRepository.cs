using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> CountProvidersAsync()
        {
            return await _context.ProviderProfiles.CountAsync();
        }

        public async Task<int> CountOrdersAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> CountOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders.CountAsync(o => o.Status == status);
        }

        public async Task<int> CountPendingSubscriptionRequestsAsync()
        {
            return await _context.SubscriptionPaymentRequests
                .CountAsync(r => r.Status == SubscriptionPaymentRequestStatus.Pending);
        }

        public async Task<int> CountActiveSubscriptionsAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.ProviderSubscriptions
                .CountAsync(s => s.StartsAt <= now && s.EndsAt > now);
        }

        public async Task<PagedResult<User>> GetUsersAsync(PaginationParams paginationParams)
        {
            var query = _context.Users
                .AsNoTracking()
                .Include(u => u.ProviderProfile)
                .OrderByDescending(u => u.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);
        }

        public async Task<PagedResult<ProviderProfile>> GetProvidersAsync(PaginationParams paginationParams)
        {
            var query = _context.ProviderProfiles
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Specialization)
                .Include(p => p.Subscriptions)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);
        }

        public async Task<PagedResult<Order>> GetOrdersAsync(PaginationParams paginationParams)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Specialization)
                .Include(o => o.SelectedProviderProfile)
                    .ThenInclude(p => p!.User)
                .Include(o => o.Rating)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);
        }
    }
}