namespace HomeMaintenanceAPI.Domain.Enums
{
    public enum OrderStatus
    {
        WaitingForOffers = 0,
        InspectionAccepted = 1,
        InProgress = 2,
        CompletionPending = 3,
        Completed = 4,
        Cancelled = 5,
        InspectionInProgress = 6
    }
}
