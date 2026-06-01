namespace HomeMaintenanceAPI.Domain.Enums
{
    public enum OfferStatus
    {
        Pending = 0,
        AcceptedForInspection = 1,
        AcceptedForWork = 2,
        Rejected = 3,
        RejectedAfterInspection = 4,
        RejectedAutomatically = 5,
        CancelledByProvider = 6,
        CancelledDueToOrderCancellation = 7
    }
}
