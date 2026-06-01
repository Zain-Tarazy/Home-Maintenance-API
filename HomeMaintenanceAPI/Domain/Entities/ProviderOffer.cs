using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Domain.Entities
{
    public class ProviderOffer
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProviderProfileId { get; set; }

        public decimal InspectionPrice { get; set; }

        public string? Note { get; set; }

        public decimal ProviderLatitude { get; set; }

        public decimal ProviderLongitude { get; set; }

        public OfferStatus Status { get; set; } = OfferStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;

        public ProviderProfile ProviderProfile { get; set; } = null!;

        public List<Notification> Notifications { get; set; } = new();
    }
}
