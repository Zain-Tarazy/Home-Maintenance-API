namespace HomeMaintenanceAPI.Application.DTOs.ProviderProfiles
{
    public class ProviderProfileDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int SpecializationId { get; set; }

        public string SpecializationName { get; set; } = string.Empty;
        public bool SpecializationIsActive { get; set; }

        public string? Bio { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
