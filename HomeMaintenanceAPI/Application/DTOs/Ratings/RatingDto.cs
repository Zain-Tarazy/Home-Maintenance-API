namespace HomeMaintenanceAPI.Application.DTOs.Ratings
{
    public class RatingDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int ProviderProfileId { get; set; }

        public string ProviderName { get; set; } = string.Empty;

        public int Value { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
