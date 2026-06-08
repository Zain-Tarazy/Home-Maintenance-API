namespace HomeMaintenanceAPI.Domain.Enums
{
    public enum NotificationType
    {
        NewOfferReceived = 0,
        OfferAcceptedForInspection = 1,
        OfferRejectedAfterInspection = 2,
        OfferAcceptedForWork = 3,
        OrderCompletionPending = 4,
        OrderCompleted = 5,
        SubscriptionApproved = 6,
        SubscriptionRejected = 7,
        OrderCancelled = 8,
        OfferRejected = 9,
        OfferRejectedAutomatically = 10,
        OfferCancelledByProvider = 11,
        RatingReceived = 12
    }
}
