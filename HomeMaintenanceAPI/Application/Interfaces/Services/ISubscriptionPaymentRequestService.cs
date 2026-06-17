using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface ISubscriptionPaymentRequestService
    {
        Task<ServiceResult<SubscriptionPaymentRequest>> CreateAsync(
            int userId,
            CreateSubscriptionPaymentRequestDto dto);

        Task<ServiceResult<List<SubscriptionPaymentRequest>>> GetMineAsync(int userId);

        Task<List<SubscriptionPaymentRequest>> GetPendingAsync();

        Task<List<SubscriptionPaymentRequest>> GetAllAsync();

        Task<ServiceResult<SubscriptionPaymentRequest>> ApproveAsync(int requestId, int adminId);

        Task<ServiceResult<SubscriptionPaymentRequest>> RejectAsync(
            int requestId,
            int adminId,
            RejectSubscriptionPaymentRequestDto dto);
    }
}
