using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Orders;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<ServiceResult<Order>> CreateAsync(int customerId, CreateOrderDto dto);

        Task<ServiceResult<PagedResult<Order>>> GetMineAsync(int customerId, PaginationParams paginationParams);

        Task<ServiceResult<Order>> GetByIdAsync(int currentUserId, int orderId);

        Task<ServiceResult<Order>> UpdateAsync(int customerId, int orderId, UpdateOrderDto dto);

        Task<ServiceResult> CancelAsync(int customerId, int orderId);

        Task<ServiceResult<PagedResult<Order>>> GetAvailableForProviderAsync(int userId, PaginationParams paginationParams);

        Task<ServiceResult<PagedResult<Order>>> GetAssignedForProviderAsync(int userId, PaginationParams paginationParams);
    }
}