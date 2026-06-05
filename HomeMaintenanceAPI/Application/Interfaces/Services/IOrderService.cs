using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Orders;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<ServiceResult<Order>> CreateAsync(int customerId, CreateOrderDto dto);

        Task<ServiceResult<List<Order>>> GetMineAsync(int customerId);

        Task<ServiceResult<Order>> GetByIdAsync(int currentUserId, int orderId);

        Task<ServiceResult<Order>> UpdateAsync(int customerId, int orderId, UpdateOrderDto dto);

        Task<ServiceResult> CancelAsync(int customerId, int orderId);

        Task<ServiceResult<List<Order>>> GetAvailableForProviderAsync(int userId);

        Task<ServiceResult<List<Order>>> GetAssignedForProviderAsync(int userId);
    }
}