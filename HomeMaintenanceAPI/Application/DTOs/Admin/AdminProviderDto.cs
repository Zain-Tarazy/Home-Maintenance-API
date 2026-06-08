namespace HomeMaintenanceAPI.Application.DTOs.Admin
{
    public class AdminProviderDto
    {
        public int ProviderProfileId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int SpecializationId { get; set; }

        public string SpecializationName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        public bool HasActiveSubscription { get; set; }

        public DateTime? ActiveSubscriptionEndsAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
