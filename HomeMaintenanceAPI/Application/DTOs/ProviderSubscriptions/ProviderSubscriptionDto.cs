namespace HomeMaintenanceAPI.Application.DTOs.ProviderSubscriptions
{
    public class ProviderSubscriptionDto
    {
        public int Id { get; set; }

        public int SubscriptionPlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public decimal PlanPrice { get; set; }

        public int DurationInDays { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } 
    }
}
