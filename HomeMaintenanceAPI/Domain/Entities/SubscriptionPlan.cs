namespace HomeMaintenanceAPI.Domain.Entities
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<SubscriptionPaymentRequest> SubscriptionPaymentRequests { get; set; } = new();

        public List<ProviderSubscription> ProviderSubscriptions { get; set; } = new();
    }
}
