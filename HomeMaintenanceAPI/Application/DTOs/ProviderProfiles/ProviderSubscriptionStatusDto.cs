namespace HomeMaintenanceAPI.Application.DTOs.ProviderProfiles
{
    public class ProviderSubscriptionStatusDto
    {
        public bool HasActiveSubscription { get; set; }

        public DateTime? ActiveSubscriptionEndsAt { get; set; }
    }
}
