using System.ComponentModel.DataAnnotations;

namespace HomeMaintenanceAPI.Domain.Entities
{
    public class Rating
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int ProviderProfileId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating value must be between 1 and 5.")]
        public int Value { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Order Order { get; set; } = null!;

        public User Customer { get; set; } = null!;

        public ProviderProfile ProviderProfile { get; set; } = null!;
    }
}
