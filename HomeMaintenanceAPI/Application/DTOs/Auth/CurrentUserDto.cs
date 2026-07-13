namespace HomeMaintenanceAPI.Application.DTOs.Auth
{
    public class CurrentUserDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool HasProviderProfile { get; set; }

        public int? ProviderProfileId { get; set; }

        public int? SpecializationId { get; set; }

        public string? SpecializationName { get; set; }

        public bool? SpecializationIsActive { get; set; }

        public bool HasActiveSubscription { get; set; }

        public DateTime? ActiveSubscriptionEndsAt { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
