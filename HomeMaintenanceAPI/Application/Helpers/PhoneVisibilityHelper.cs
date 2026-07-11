using HomeMaintenanceAPI.Application.DTOs.Offers;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Helpers
{
    public static class PhoneVisibilityHelper
    {
        public static void ApplyForProviderViewingOwnOffer(OfferDto offer)
        {
            var offerIsSelected =
                offer.Status == OfferStatus.AcceptedForInspection ||
                offer.Status == OfferStatus.AcceptedForWork;

            var orderStatusAllowsPhone =
                offer.OrderStatus == OrderStatus.InspectionAccepted ||
                offer.OrderStatus == OrderStatus.InProgress ||
                offer.OrderStatus == OrderStatus.CompletionPending ||
                offer.OrderStatus == OrderStatus.Completed;

            var canSeeCustomerPhone = offerIsSelected && orderStatusAllowsPhone;

            if (!canSeeCustomerPhone)
            {
                offer.CustomerPhoneNumber = null;
            }

            // In "my offers", the provider does not need his own phone returned.
            offer.ProviderPhoneNumber = null;
        }
    }
}
