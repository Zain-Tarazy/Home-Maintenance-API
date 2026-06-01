namespace HomeMaintenanceAPI.Domain.Entities
{
    public class ProviderProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int SpecializationId { get; set; }

        public string? Bio { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;

        public Specialization Specialization { get; set; } = null!;

        public List<ProviderOffer> Offers { get; set; } = new();

        public List<Rating> Ratings { get; set; } = new();

        public List<SubscriptionPaymentRequest> SubscriptionPaymentRequests { get; set; } = new();

        public List<ProviderSubscription> Subscriptions { get; set; } = new();

        public List<Order> SelectedOrders { get; set; } = new();
    }
}
