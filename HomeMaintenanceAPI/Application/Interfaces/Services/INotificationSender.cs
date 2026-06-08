using HomeMaintenanceAPI.Application.DTOs.Notifications;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface INotificationSender
    {
        Task SendToUserAsync(int userId, NotificationDto notification);
    }
}
