using System.Security.Claims;
using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.Notifications;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationsController(
            INotificationService notificationService,
            IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();

            var result = await _notificationService.GetMineAsync(userId);

            var response = _mapper.Map<List<NotificationDto>>(result.Data!);

            return Ok(response);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _notificationService.MarkAsReadAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Notification marked as read.");
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();

            var result = await _notificationService.MarkAllAsReadAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("All notifications marked as read.");
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }
    }
}