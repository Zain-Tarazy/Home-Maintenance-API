using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Admin;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardSummaryDto> GetDashboardSummaryAsync();

        Task<PagedResult<User>> GetUsersAsync(PaginationParams paginationParams);

        Task<PagedResult<ProviderProfile>> GetProvidersAsync(PaginationParams paginationParams);

        Task<PagedResult<Order>> GetOrdersAsync(PaginationParams paginationParams);
    }
}