namespace HomeMaintenanceAPI.Application.DTOs.OrderInspection
{
    public class GenerateInspectionQrResponseDto
    {
        public int OrderId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
