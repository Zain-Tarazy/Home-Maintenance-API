namespace HomeMaintenanceAPI.Application.DTOs.OrderCompletion
{
    public class CompleteOrderByQrDto
    {
        public int OrderId { get; set; }

        public string Token { get; set; } = string.Empty;
    }
}
