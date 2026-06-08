namespace HomeMaintenanceAPI.Application.DTOs.Admin
{
    public class AdminDashboardSummaryDto
    {
        public int TotalUsers { get; set; }

        public int TotalProviders { get; set; }

        public int TotalOrders { get; set; }

        public int WaitingForOffersOrders { get; set; }

        public int InProgressOrders { get; set; }

        public int CompletedOrders { get; set; }

        public int PendingSubscriptionRequests { get; set; }

        public int ActiveSubscriptions { get; set; }
    }
}
