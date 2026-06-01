namespace HomeMaintenanceAPI.Domain.Entities
{
    public class ProviderSubscription
    {
        public int Id { get; set; }

        public int ProviderProfileId { get; set; }

        public int SubscriptionPlanId { get; set; }

        public int SubscriptionPaymentRequestId { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ProviderProfile ProviderProfile { get; set; } = null!;

        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

        public SubscriptionPaymentRequest SubscriptionPaymentRequest { get; set; } = null!;
    }
}
