using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        public int? RelatedOrderId { get; set; }

        public int? RelatedOfferId { get; set; }

        public int? RelatedSubscriptionPaymentRequestId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;

        public Order? RelatedOrder { get; set; }

        public ProviderOffer? RelatedOffer { get; set; }

        public SubscriptionPaymentRequest? RelatedSubscriptionPaymentRequest { get; set; }
    }
}
