using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Domain.Entities
{
    public class SubscriptionPaymentRequest
    {
        public int Id { get; set; }

        public int ProviderProfileId { get; set; }

        public int SubscriptionPlanId { get; set; }

        //public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public string? ProofImageUrl { get; set; }

        public SubscriptionPaymentRequestStatus Status { get; set; } =
            SubscriptionPaymentRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByAdminId { get; set; }

        public string? AdminNote { get; set; }

        // Navigation properties
        public ProviderProfile ProviderProfile { get; set; } = null!;

        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

        public User? ReviewedByAdmin { get; set; }

        public ProviderSubscription? ProviderSubscription { get; set; }

        public List<Notification> Notifications { get; set; } = new();
    }
}
