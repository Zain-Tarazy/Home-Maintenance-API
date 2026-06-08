using HomeMaintenanceAPI.Application.DTOs.Notifications;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HomeMaintenanceAPI.Infrastructure.Services
{
    public class SignalRNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public SignalRNotificationSender(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(int userId, NotificationDto notification)
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }
    }
}