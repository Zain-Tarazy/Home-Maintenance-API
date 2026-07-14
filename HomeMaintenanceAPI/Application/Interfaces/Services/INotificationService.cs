using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<PagedResult<Notification>> GetMineAsync(int userId, PaginationParams paginationParams);

        Task<ServiceResult> MarkAsReadAsync(int userId, int notificationId);
        Task<ServiceResult> MarkAllAsReadAsync(int userId);

        Task CreateAndSendAsync(
            int userId,
            string title,
            string message,
            NotificationType type,
            int? relatedOrderId = null,
            int? relatedOfferId = null,
            int? relatedSubscriptionPaymentRequestId = null);
    }
}
