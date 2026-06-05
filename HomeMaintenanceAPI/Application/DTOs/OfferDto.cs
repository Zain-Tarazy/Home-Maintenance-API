using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.DTOs
{
    public class OfferDto
    {
            public int Id { get; set; }

            public int OrderId { get; set; }

            public string OrderDescription { get; set; } = string.Empty;

            public OrderStatus OrderStatus { get; set; }

            public int ProviderProfileId { get; set; }

            public string ProviderName { get; set; } = string.Empty;

            public string? ProviderPhoneNumber { get; set; }

            public string SpecializationName { get; set; } = string.Empty;

            public string? CustomerName { get; set; }

            public string? CustomerPhoneNumber { get; set; }

            public decimal InspectionPrice { get; set; }

            public string? Note { get; set; }

            public decimal ProviderLatitude { get; set; }

            public decimal ProviderLongitude { get; set; }

            public OfferStatus Status { get; set; }

            public double AverageRating { get; set; }

            public int RatingsCount { get; set; }

            public int CompletedOrdersCount { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime? UpdatedAt { get; set; }
        }
    }
