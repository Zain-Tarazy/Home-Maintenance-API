using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.DTOs.Admin
{
    public class AdminUserDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool HasProviderProfile { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
