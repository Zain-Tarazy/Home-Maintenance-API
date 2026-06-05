namespace HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
