namespace HomeMaintenanceAPI.Application.DTOs.Orders
{
    public class UpdateOrderDto
    {
        public int SpecializationId { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string? AddressText { get; set; }
    }
}
