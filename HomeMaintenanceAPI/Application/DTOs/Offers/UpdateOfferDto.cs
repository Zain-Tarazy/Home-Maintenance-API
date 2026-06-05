namespace HomeMaintenanceAPI.Application.DTOs.Offers
{
    public class UpdateOfferDto
    {
        public decimal InspectionPrice { get; set; }

        public string? Note { get; set; }

        public decimal ProviderLatitude { get; set; }

        public decimal ProviderLongitude { get; set; }
    }
}
