using AutoMapper;
using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Notifications;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationSender _notificationSender;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(

            INotificationRepository notificationRepository,
            INotificationSender notificationSender,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _notificationSender = notificationSender;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<Notification>> GetMineAsync(
            int userId,
            PaginationParams paginationParams)
        {
            return await _notificationRepository.GetByUserIdAsync(
                userId,
                paginationParams);
        }

        public async Task<ServiceResult> MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
                return ServiceResult.Failure("Notification not found.");

            if (notification.UserId != userId)
                return ServiceResult.Failure("You are not allowed to update this notification.");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;

                await _notificationRepository.UpdateAsync(notification);
                await _notificationRepository.SaveChangesAsync();
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications =
                await _notificationRepository.GetUnreadByUserIdAsync(userId);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _notificationRepository.UpdateRangeAsync(unreadNotifications);
            await _notificationRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task CreateAndSendAsync(
            int userId,
            string title,
            string message,
            NotificationType type,
            int? relatedOrderId = null,
            int? relatedOfferId = null,
            int? relatedSubscriptionPaymentRequestId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RelatedOrderId = relatedOrderId,
                RelatedOfferId = relatedOfferId,
                RelatedSubscriptionPaymentRequestId = relatedSubscriptionPaymentRequestId
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Notification created. NotificationId={NotificationId}, UserId={UserId}, Type={Type}, RelatedOrderId={RelatedOrderId}, RelatedOfferId={RelatedOfferId}, RelatedSubscriptionPaymentRequestId={RelatedSubscriptionPaymentRequestId}",
                notification.Id,
                userId,
                type,
                relatedOrderId,
                relatedOfferId,
                relatedSubscriptionPaymentRequestId);

            var dto = _mapper.Map<NotificationDto>(notification);

            await _notificationSender.SendToUserAsync(userId, dto);
            _logger.LogInformation(
                "Notification sent through SignalR. NotificationId={NotificationId}, UserId={UserId}",
                notification.Id,
                userId);
        }
    }
}
