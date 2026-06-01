using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int SpecializationId { get; set; }

        public int? SelectedProviderProfileId { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string? AddressText { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.WaitingForOffers;

        public string? CompletionTokenHash { get; set; }

        public DateTime? CompletionTokenExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public User Customer { get; set; } = null!;

        public Specialization Specialization { get; set; } = null!;

        public ProviderProfile? SelectedProviderProfile { get; set; }

        public List<ProviderOffer> Offers { get; set; } = new();

        public Rating? Rating { get; set; }

        public List<Notification> Notifications { get; set; } = new();
    }
}
