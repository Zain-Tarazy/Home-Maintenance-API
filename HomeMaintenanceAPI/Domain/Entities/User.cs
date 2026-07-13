using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;

        public bool IsEmailVerified { get; set; } = false;

        public string? EmailVerificationCodeHash { get; set; }

        public DateTime? EmailVerificationCodeExpiresAt { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? PasswordResetCodeHash { get; set; }

        public DateTime? PasswordResetCodeExpiresAt { get; set; }

        public string? ProfileImageUrl { get; set; }

        // Navigation properties
        public ProviderProfile? ProviderProfile { get; set; }

        public List<Order> Orders { get; set; } = new();

        public List<Rating> Ratings { get; set; } = new();

        public List<Notification> Notifications { get; set; } = new();

        public List<SubscriptionPaymentRequest> ReviewedSubscriptionPaymentRequests { get; set; } = new();
    }
}
