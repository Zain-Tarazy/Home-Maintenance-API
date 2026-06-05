namespace HomeMaintenanceAPI.Application.DTOs.Ratings
{
    public class ProviderRatingSummaryDto
    {
        public int ProviderProfileId { get; set; }

        public double AverageRating { get; set; }

        public int RatingsCount { get; set; }

        public int CompletedOrdersCount { get; set; }
    }
}
