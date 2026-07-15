namespace HomeMaintenanceAPI.Application.DTOs.OrderInspection
{
    public class ConfirmInspectionByQrDto
    {
        public int OrderId { get; set; }

        public string Token { get; set; } = string.Empty;
    }
}
