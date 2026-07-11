using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Offers)
                    .ThenInclude(of => of.ProviderProfile)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        } 

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Specialization)
                .Include(o => o.SelectedProviderProfile)
                    .ThenInclude(p => p!.User)
                .Include(o => o.Offers)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<PagedResult<Order>> GetByCustomerIdAsync(
            int customerId,
            PaginationParams paginationParams)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Specialization)
                .Include(o => o.SelectedProviderProfile)
                    .ThenInclude(p => p!.User)
                .Include(o => o.Rating)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);

        }

        public async Task<PagedResult<Order>> GetAvailableForProviderAsync(
            int providerUserId,
            int specializationId,
            PaginationParams paginationParams)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Specialization)
                .Include(o => o.SelectedProviderProfile)
                    .ThenInclude(p => p!.User)
                .Where(o =>
                    o.Status == OrderStatus.WaitingForOffers &&
                    o.SpecializationId == specializationId &&
                    o.CustomerId != providerUserId)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);
        }

        public async Task<PagedResult<Order>> GetAssignedForProviderAsync(
            int providerProfileId,
            PaginationParams paginationParams)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Specialization)
                .Include(o => o.SelectedProviderProfile)
                    .ThenInclude(p => p!.User)
                .Include(o => o.Rating)
                .Where(o =>
                    o.SelectedProviderProfileId == providerProfileId &&
                    o.Status != OrderStatus.Cancelled)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(paginationParams);
        }

        public async Task<bool> HasOffersAsync(int orderId)
        {
            return await _context.ProviderOffers
                .AnyAsync(o => o.OrderId == orderId);
        }

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
        }

        public Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
