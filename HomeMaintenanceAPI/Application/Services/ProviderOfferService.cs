using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Offers;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using HomeMaintenanceAPI.Infrastructure.Repositories;

namespace HomeMaintenanceAPI.Application.Services
{
    public class ProviderOfferService : IProviderOfferService
    {
        private readonly IProviderOfferRepository _offerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly INotificationService _notificationService;
        private readonly ISpecializationRepository _specializationRepository;


        public ProviderOfferService(
            IProviderOfferRepository offerRepository,
            IOrderRepository orderRepository,
            IProviderProfileRepository providerProfileRepository,
            INotificationService notificationService,
            ISpecializationRepository specializationRepository)
        {
            _offerRepository = offerRepository;
            _orderRepository = orderRepository;
            _providerProfileRepository = providerProfileRepository;
            _notificationService = notificationService;
            _specializationRepository = specializationRepository;
        }

        public async Task<ServiceResult<ProviderOffer>> CreateAsync(int userId, CreateOfferDto dto)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult<ProviderOffer>.Failure("Provider profile not found.");

            var providerSpecialization = await _specializationRepository.GetByIdAsync(providerProfile.SpecializationId);    

            if (providerSpecialization == null)
                return ServiceResult<ProviderOffer>.Failure("Provider specialization not found.");

            if (!providerSpecialization.IsActive)
                return ServiceResult<ProviderOffer>.Failure(
                    "Your specialization is currently inactive. You cannot submit offers until it is reactivated by the admin.");

            if (providerProfile == null)
                return ServiceResult<ProviderOffer>.Failure("Provider profile not found.");

            var order = await _orderRepository.GetByIdWithDetailsAsync(dto.OrderId);

            if (order == null)
                return ServiceResult<ProviderOffer>.Failure("Order not found.");

            if (order.Status != OrderStatus.WaitingForOffers)
                return ServiceResult<ProviderOffer>.Failure("Offers can only be submitted to orders waiting for offers.");

            if (order.CustomerId == userId)
                return ServiceResult<ProviderOffer>.Failure("You cannot submit an offer to your own order.");

            if (providerProfile.SpecializationId != order.SpecializationId)
                return ServiceResult<ProviderOffer>.Failure("Your specialization does not match this order.");

            var hasActiveSubscription =
                await _providerProfileRepository.HasActiveSubscriptionAsync(providerProfile.Id);

            if (!hasActiveSubscription)
                return ServiceResult<ProviderOffer>.Failure("You need an active subscription to submit offers.");

            if (await _offerRepository.ExistsForOrderAndProviderAsync(order.Id, providerProfile.Id))
                return ServiceResult<ProviderOffer>.Failure("You already submitted an offer to this order.");

            var validation = ValidateOfferInput(dto.InspectionPrice, dto.ProviderLatitude, dto.ProviderLongitude);

            if (!validation.Succeeded)
                return ServiceResult<ProviderOffer>.Failure(validation.Error!);

            var offer = new ProviderOffer
            {
                OrderId = order.Id,
                ProviderProfileId = providerProfile.Id,
                InspectionPrice = dto.InspectionPrice,
                Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim(),
                ProviderLatitude = dto.ProviderLatitude,
                ProviderLongitude = dto.ProviderLongitude,
                Status = OfferStatus.Pending
            };

            await _offerRepository.AddAsync(offer);
            await _offerRepository.SaveChangesAsync();

            var createdOffer = await _offerRepository.GetByIdWithDetailsAsync(offer.Id);

            await _notificationService.CreateAndSendAsync(
                order.CustomerId,
                "New offer received",
                "A provider submitted an offer for your order.",
                NotificationType.NewOfferReceived,
                relatedOrderId: order.Id,
                relatedOfferId: createdOffer!.Id);

            return ServiceResult<ProviderOffer>.Success(createdOffer!);

            // Later: notify customer NewOfferReceived
        }

        public async Task<ServiceResult<PagedResult<ProviderOffer>>> GetMineAsync(
            int providerUserId,
            PaginationParams paginationParams)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(providerUserId);

            if (providerProfile == null)
                return ServiceResult<PagedResult<ProviderOffer>>.Failure("Provider profile not found.");

            var offers = await _offerRepository.GetByProviderProfileIdAsync(
                providerProfile.Id,
                paginationParams);

            return ServiceResult<PagedResult<ProviderOffer>>.Success(offers);
        }

        public async Task<ServiceResult<List<ProviderOffer>>> GetByOrderIdAsync(int userId, int orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null)
                return ServiceResult<List<ProviderOffer>>.Failure("Order not found.");

            if (order.CustomerId != userId)
                return ServiceResult<List<ProviderOffer>>.Failure("You are not allowed to view offers for this order.");

            var offers = await _offerRepository.GetByOrderIdAsync(orderId);

            return ServiceResult<List<ProviderOffer>>.Success(offers);
        }

        public async Task<ServiceResult<ProviderOffer>> UpdateAsync(int userId, int offerId, UpdateOfferDto dto)
        {
            var offer = await _offerRepository.GetByIdWithDetailsAsync(offerId);

            if (offer == null)
                return ServiceResult<ProviderOffer>.Failure("Offer not found.");

            if (offer.ProviderProfile.UserId != userId)
                return ServiceResult<ProviderOffer>.Failure("You are not allowed to update this offer.");

            if (offer.Status != OfferStatus.Pending)
                return ServiceResult<ProviderOffer>.Failure("Only pending offers can be updated.");

            if (offer.Order.Status != OrderStatus.WaitingForOffers)
                return ServiceResult<ProviderOffer>.Failure("Cannot update offer because the order is no longer waiting for offers.");

            var validation = ValidateOfferInput(dto.InspectionPrice, dto.ProviderLatitude, dto.ProviderLongitude);

            if (!validation.Succeeded)
                return ServiceResult<ProviderOffer>.Failure(validation.Error!);

            offer.InspectionPrice = dto.InspectionPrice;
            offer.Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();
            offer.ProviderLatitude = dto.ProviderLatitude;
            offer.ProviderLongitude = dto.ProviderLongitude;
            offer.UpdatedAt = DateTime.UtcNow;

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.SaveChangesAsync();

            var updatedOffer = await _offerRepository.GetByIdWithDetailsAsync(offer.Id);

            return ServiceResult<ProviderOffer>.Success(updatedOffer!);
        }

        public async Task<ServiceResult> CancelAsync(int userId, int offerId)
        {
            var offer = await _offerRepository.GetByIdWithDetailsAsync(offerId);

            if (offer == null)
                return ServiceResult.Failure("Offer not found.");

            if (offer.ProviderProfile.UserId != userId)
                return ServiceResult.Failure("You are not allowed to cancel this offer.");

            if (offer.Status != OfferStatus.Pending)
                return ServiceResult.Failure("Only pending offers can be cancelled.");

            if (offer.Order.Status != OrderStatus.WaitingForOffers)
                return ServiceResult.Failure("Cannot cancel offer because the order is no longer waiting for offers.");

            offer.Status = OfferStatus.CancelledByProvider;
            offer.UpdatedAt = DateTime.UtcNow;

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                offer.Order.CustomerId,
                "Offer cancelled",
                "A provider cancelled their offer on your order.",
                NotificationType.OfferCancelledByProvider,
                relatedOrderId: offer.OrderId,
                relatedOfferId: offer.Id);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectAsync(int userId, int offerId)
        {
            var offer = await _offerRepository.GetByIdWithDetailsAsync(offerId);

            if (offer == null)
                return ServiceResult.Failure("Offer not found.");

            if (offer.Order.CustomerId != userId)
                return ServiceResult.Failure("You are not allowed to reject this offer.");

            if (offer.Order.Status != OrderStatus.WaitingForOffers)
                return ServiceResult.Failure("Offers can only be rejected before inspection is accepted.");

            if (offer.Status != OfferStatus.Pending)
                return ServiceResult.Failure("Only pending offers can be rejected.");

            offer.Status = OfferStatus.Rejected;
            offer.UpdatedAt = DateTime.UtcNow;

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                offer.ProviderProfile.UserId,
                "Offer rejected",
                "The customer rejected your offer.",
                NotificationType.OfferRejected,
                relatedOrderId: offer.OrderId,
                relatedOfferId: offer.Id);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<ProviderOffer>> AcceptForInspectionAsync(int userId, int offerId)
        {
            var offer = await _offerRepository.GetByIdWithDetailsAsync(offerId);

            if (offer == null)
                return ServiceResult<ProviderOffer>.Failure("Offer not found.");

            var order = offer.Order;

            if (order.CustomerId != userId)
                return ServiceResult<ProviderOffer>.Failure("You are not allowed to accept this offer.");

            if (order.Status != OrderStatus.WaitingForOffers)
                return ServiceResult<ProviderOffer>.Failure("Order is not waiting for offers.");

            if (offer.Status != OfferStatus.Pending)
                return ServiceResult<ProviderOffer>.Failure("Only pending offers can be accepted.");

            if (await _offerRepository.HasActiveAcceptedOfferAsync(order.Id))
                return ServiceResult<ProviderOffer>.Failure("There is already an active accepted offer for this order.");

            order.Status = OrderStatus.InspectionAccepted;
            order.SelectedProviderProfileId = offer.ProviderProfileId;
            order.UpdatedAt = DateTime.UtcNow;

            offer.Status = OfferStatus.AcceptedForInspection;
            offer.UpdatedAt = DateTime.UtcNow;

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                offer.ProviderProfile.UserId,
                "Offer accepted for inspection",
                "Your offer was accepted for inspection.",
                NotificationType.OfferAcceptedForInspection,
                relatedOrderId: order.Id,
                relatedOfferId: offer.Id);

            var updatedOffer = await _offerRepository.GetByIdWithDetailsAsync(offer.Id);

            return ServiceResult<ProviderOffer>.Success(updatedOffer!);

            // Later: notify provider OfferAcceptedForInspection
        }

        public async Task<ServiceResult<ProviderOffer>> RejectAfterInspectionAsync(int userId, int offerId)
        {
            var offer = await _offerRepository.GetByIdWithDetailsAsync(offerId);

            if (offer == null)
                return ServiceResult<ProviderOffer>.Failure("Offer not found.");

            var order = offer.Order;

            if (order.CustomerId != userId)
                return ServiceResult<ProviderOffer>.Failure("You are not allowed to reject this provider.");

            if (order.Status != OrderStatus.InspectionAccepted)
                return ServiceResult<ProviderOffer>.Failure("Order is not in inspection stage.");

            if (offer.Status != OfferStatus.AcceptedForInspection)
                return ServiceResult<ProviderOffer>.Failure("This offer is not accepted for inspection.");

            if (order.SelectedProviderProfileId != offer.ProviderProfileId)
                return ServiceResult<ProviderOffer>.Failure("This provider is not the selected provider for this order.");

            order.Status = OrderStatus.WaitingForOffers;
            order.SelectedProviderProfileId = null;
            order.UpdatedAt = DateTime.UtcNow;

            offer.Status = OfferStatus.RejectedAfterInspection;
            offer.UpdatedAt = DateTime.UtcNow;

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                offer.ProviderProfile.UserId,
                "Offer rejected after inspection",
                "The customer rejected your offer after inspection.",
                NotificationType.OfferRejectedAfterInspection,
                relatedOrderId: order.Id,
                relatedOfferId: offer.Id);

            var updatedOffer = await _offerRepository.GetByIdWithDetailsAsync(offer.Id);

            return ServiceResult<ProviderOffer>.Success(updatedOffer!);

            // Later: notify provider OfferRejectedAfterInspection
        }

        public async Task<ServiceResult<ProviderOffer>> ContinueWorkAsync(int userId, int offerId)
        {
            var offer = await _offerRepository.GetByIdWithDetailsAsync(offerId);

            if (offer == null)
                return ServiceResult<ProviderOffer>.Failure("Offer not found.");

            var order = offer.Order;

            if (order.CustomerId != userId)
                return ServiceResult<ProviderOffer>.Failure("You are not allowed to continue with this provider.");

            if (order.Status != OrderStatus.InspectionAccepted)
                return ServiceResult<ProviderOffer>.Failure("Order is not in inspection stage.");

            if (offer.Status != OfferStatus.AcceptedForInspection)
                return ServiceResult<ProviderOffer>.Failure("This offer is not accepted for inspection.");

            if (order.SelectedProviderProfileId != offer.ProviderProfileId)
                return ServiceResult<ProviderOffer>.Failure("This provider is not selected for this order.");

            order.Status = OrderStatus.InProgress;
            order.UpdatedAt = DateTime.UtcNow;

            offer.Status = OfferStatus.AcceptedForWork;
            offer.UpdatedAt = DateTime.UtcNow;

            var otherPendingOffers =
                await _offerRepository.GetPendingOffersForOrderExceptAsync(order.Id, offer.Id);

            foreach (var pendingOffer in otherPendingOffers)
            {
                pendingOffer.Status = OfferStatus.RejectedAutomatically;
                pendingOffer.UpdatedAt = DateTime.UtcNow;
            }

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.UpdateRangeAsync(otherPendingOffers);
            await _offerRepository.SaveChangesAsync();

            foreach (var pendingOffer in otherPendingOffers)
            {
                await _notificationService.CreateAndSendAsync(
                    pendingOffer.ProviderProfile.UserId,
                    "Offer rejected automatically",
                    "The customer continued with another provider for this order.",
                    NotificationType.OfferRejectedAutomatically,
                    relatedOrderId: order.Id,
                    relatedOfferId: pendingOffer.Id);
            }

            var updatedOffer = await _offerRepository.GetByIdWithDetailsAsync(offer.Id);

            return ServiceResult<ProviderOffer>.Success(updatedOffer!);

            // Later: notify provider OfferAcceptedForWork
        }

        private ServiceResult ValidateOfferInput(decimal inspectionPrice, decimal latitude, decimal longitude)
        {
            if (inspectionPrice < 0)
                return ServiceResult.Failure("Inspection price cannot be negative.");

            if (latitude < -90 || latitude > 90)
                return ServiceResult.Failure("Latitude must be between -90 and 90.");

            if (longitude < -180 || longitude > 180)
                return ServiceResult.Failure("Longitude must be between -180 and 180.");

            return ServiceResult.Success();
        }
    }
}
