namespace HomeMaintenanceAPI.Application.Helpers
{
    public static class LocationValidationHelper
    {
        // Approximate wider Damascus service area.
        // Adjust later if needed.
        private const decimal MinLatitude = 33.30m;
        private const decimal MaxLatitude = 33.70m;

        private const decimal MinLongitude = 36.05m;
        private const decimal MaxLongitude = 36.60m;

        public static bool IsWithinServiceArea(decimal latitude, decimal longitude)
        {
            return latitude >= MinLatitude &&
                   latitude <= MaxLatitude &&
                   longitude >= MinLongitude &&
                   longitude <= MaxLongitude;
        }
    }
}
