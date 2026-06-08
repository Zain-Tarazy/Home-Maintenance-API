using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.DTOs.Admin
{
    public class AdminOrderDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int SpecializationId { get; set; }

        public string SpecializationName { get; set; } = string.Empty;

        public int? SelectedProviderProfileId { get; set; }

        public string? SelectedProviderName { get; set; }

        public OrderStatus Status { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
