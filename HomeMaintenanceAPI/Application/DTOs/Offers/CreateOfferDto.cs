namespace HomeMaintenanceAPI.Application.DTOs.Offers
{
    public class CreateOfferDto
    {
        public int OrderId { get; set; }

        public decimal InspectionPrice { get; set; }

        public string? Note { get; set; }

        public decimal ProviderLatitude { get; set; }

        public decimal ProviderLongitude { get; set; }
    }
}
