using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhoneNumber { get; set; }

        public int SpecializationId { get; set; }

        public string SpecializationName { get; set; } = string.Empty;

        public int? SelectedProviderProfileId { get; set; }

        public string? SelectedProviderName { get; set; }

        public string? SelectedProviderPhoneNumber { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string? AddressText { get; set; }

        public string? CustomerProfileImageUrl { get; set; }

        public string? SelectedProviderProfileImageUrl { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}