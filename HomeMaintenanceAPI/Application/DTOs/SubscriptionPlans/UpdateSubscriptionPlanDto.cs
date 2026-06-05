namespace HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans
{
    public class UpdateSubscriptionPlanDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }
    }
}
