using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeMaintenanceAPI.Presentation.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {
    }
}
