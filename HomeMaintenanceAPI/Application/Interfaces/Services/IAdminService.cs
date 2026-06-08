using HomeMaintenanceAPI.Application.DTOs.Admin;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardSummaryDto> GetDashboardSummaryAsync();

        Task<List<User>> GetUsersAsync();

        Task<List<ProviderProfile>> GetProvidersAsync();

        Task<List<Order>> GetOrdersAsync();
    }
}