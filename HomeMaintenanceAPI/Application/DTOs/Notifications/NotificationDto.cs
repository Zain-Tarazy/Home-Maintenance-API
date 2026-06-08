using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }

        public int? RelatedOrderId { get; set; }

        public int? RelatedOfferId { get; set; }

        public int? RelatedSubscriptionPaymentRequestId { get; set; }
    }
}
