namespace HomeMaintenanceAPI.Application.DTOs.OrderCompletion
{
    public class GenerateCompletionQrResponseDto
    {
        public int OrderId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}
