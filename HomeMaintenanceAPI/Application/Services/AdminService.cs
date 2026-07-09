using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Admin;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<AdminDashboardSummaryDto> GetDashboardSummaryAsync()
        {
            return new AdminDashboardSummaryDto
            {
                TotalUsers = await _adminRepository.CountUsersAsync(),
                TotalProviders = await _adminRepository.CountProvidersAsync(),
                TotalOrders = await _adminRepository.CountOrdersAsync(),
                WaitingForOffersOrders = await _adminRepository.CountOrdersByStatusAsync(OrderStatus.WaitingForOffers),
                InProgressOrders = await _adminRepository.CountOrdersByStatusAsync(OrderStatus.InProgress),
                CompletedOrders = await _adminRepository.CountOrdersByStatusAsync(OrderStatus.Completed),
                PendingSubscriptionRequests = await _adminRepository.CountPendingSubscriptionRequestsAsync(),
                ActiveSubscriptions = await _adminRepository.CountActiveSubscriptionsAsync()
            };
        }

        public async Task<PagedResult<User>> GetUsersAsync(PaginationParams paginationParams)
        {
            return await _adminRepository.GetUsersAsync(paginationParams);
        }

        public async Task<PagedResult<ProviderProfile>> GetProvidersAsync(PaginationParams paginationParams)
        {
            return await _adminRepository.GetProvidersAsync(paginationParams);
        }

        public async Task<PagedResult<Order>> GetOrdersAsync(PaginationParams paginationParams)
        {
            return await _adminRepository.GetOrdersAsync(paginationParams);
        }
    }
}