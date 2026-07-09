using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class ProviderOfferRepository : IProviderOfferRepository
    {
        private readonly AppDbContext _context;

        public ProviderOfferRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderOffer?> GetByIdWithDetailsAsync(int id)
        {
            return await IncludeDetails()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<ProviderOffer>> GetByOrderIdAsync(int orderId)
        {
            return await IncludeDetails()
                .AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResult<ProviderOffer>> GetByProviderProfileIdAsync(
            int providerProfileId,
            PaginationParams paginationParams)
        {
            var query = _context.ProviderOffers
                .AsNoTracking()
                .Include(o => o.Order)
                    .ThenInclude(order => order.Customer)
                .Include(o => o.Order)
                    .ThenInclude(order => order.Specialization)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.User)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.Specialization)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.Ratings)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.SelectedOrders)
                .Where(o => o.ProviderProfileId == providerProfileId)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return new PagedResult<ProviderOffer>(
                items,
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalCount
            );
        }

        public async Task<bool> ExistsForOrderAndProviderAsync(int orderId, int providerProfileId)
        {
            return await _context.ProviderOffers
                .AnyAsync(o =>
                    o.OrderId == orderId &&
                    o.ProviderProfileId == providerProfileId);
        }

        public async Task<bool> HasActiveAcceptedOfferAsync(int orderId)
        {
            return await _context.ProviderOffers
                .AnyAsync(o =>
                    o.OrderId == orderId &&
                    (o.Status == OfferStatus.AcceptedForInspection ||
                     o.Status == OfferStatus.AcceptedForWork));
        }

        public async Task<List<ProviderOffer>> GetPendingOffersForOrderExceptAsync(int orderId, int excludedOfferId)
        {
            return await _context.ProviderOffers
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.User)
                .Where(o =>
                    o.OrderId == orderId &&
                    o.Id != excludedOfferId &&
                    o.Status == OfferStatus.Pending)
                .ToListAsync();
        }

        public async Task<ProviderOffer> AddAsync(ProviderOffer offer)
        {
            await _context.ProviderOffers.AddAsync(offer);
            return offer;
        }

        public Task UpdateAsync(ProviderOffer offer)
        {
            _context.ProviderOffers.Update(offer);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(List<ProviderOffer> offers)
        {
            _context.ProviderOffers.UpdateRange(offers);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<ProviderOffer> IncludeDetails()
        {
            return _context.ProviderOffers
                .Include(o => o.Order)
                    .ThenInclude(order => order.Customer)
                .Include(o => o.Order)
                    .ThenInclude(order => order.Specialization)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.User)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.Specialization)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.Ratings)
                .Include(o => o.ProviderProfile)
                    .ThenInclude(p => p.SelectedOrders);
        }
    }
}
