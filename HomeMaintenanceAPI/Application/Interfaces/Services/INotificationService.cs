using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<ServiceResult<List<Notification>>> GetMineAsync(int userId);

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
