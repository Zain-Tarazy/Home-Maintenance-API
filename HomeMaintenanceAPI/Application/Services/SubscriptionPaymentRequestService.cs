using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Services
{
    public class SubscriptionPaymentRequestService : ISubscriptionPaymentRequestService
    {
        private readonly INotificationService _notificationService;
        private readonly ISubscriptionPaymentRequestRepository _requestRepository;
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

        public SubscriptionPaymentRequestService(
            ISubscriptionPaymentRequestRepository requestRepository,
            IProviderProfileRepository providerProfileRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            INotificationService notificationService)
        {
            _requestRepository = requestRepository;
            _providerProfileRepository = providerProfileRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _notificationService = notificationService;
        }
        public async Task<ServiceResult<SubscriptionPaymentRequest>> CreateAsync(
            int userId,
            CreateSubscriptionPaymentRequestDto dto)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Provider profile not found.");

            var hasActiveSubscription =
                await _providerProfileRepository.HasActiveSubscriptionAsync(providerProfile.Id);

            if (hasActiveSubscription)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("You already have an active subscription.");

            var hasPendingRequest =
                await _requestRepository.HasPendingRequestAsync(providerProfile.Id);

            if (hasPendingRequest)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("You already have a pending subscription request.");

            var plan = await _subscriptionPlanRepository.GetByIdAsync(dto.SubscriptionPlanId);

            if (plan == null)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Subscription plan not found.");

            if (!plan.IsActive)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Subscription plan is not active.");

            var transactionId = dto.TransactionId.Trim();

            if (string.IsNullOrWhiteSpace(transactionId))
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Transaction id is required.");

            if (await _requestRepository.TransactionIdExistsAsync(transactionId))
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Transaction id already exists.");

            var request = new SubscriptionPaymentRequest
            {
                ProviderProfileId = providerProfile.Id,
                SubscriptionPlanId = plan.Id,
                Amount = plan.Price,
                PaymentMethod = dto.PaymentMethod,
                TransactionId = transactionId,
                ProofImageUrl = string.IsNullOrWhiteSpace(dto.ProofImageUrl)
                    ? null
                    : dto.ProofImageUrl.Trim(),
                Status = SubscriptionPaymentRequestStatus.Pending
            };

            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangesAsync();

            var createdRequest = await _requestRepository.GetByIdAsync(request.Id);

            return ServiceResult<SubscriptionPaymentRequest>.Success(createdRequest!);
        }

        public async Task<ServiceResult<List<SubscriptionPaymentRequest>>> GetMineAsync(int userId)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult<List<SubscriptionPaymentRequest>>.Failure("Provider profile not found.");

            var requests = await _requestRepository.GetByProviderProfileIdAsync(providerProfile.Id);

            return ServiceResult<List<SubscriptionPaymentRequest>>.Success(requests);
        }

        public async Task<List<SubscriptionPaymentRequest>> GetPendingAsync()
        {
            return await _requestRepository.GetPendingAsync();
        }
        public async Task<List<SubscriptionPaymentRequest>> GetAllAsync()
        {
            return await _requestRepository.GetAllAsync();
        }

        public async Task<ServiceResult<SubscriptionPaymentRequest>> ApproveAsync(int requestId, int adminId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);

            if (request == null)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Subscription payment request not found.");

            if (request.Status != SubscriptionPaymentRequestStatus.Pending)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Only pending requests can be approved.");

            var hasActiveSubscription =
                await _providerProfileRepository.HasActiveSubscriptionAsync(request.ProviderProfileId);

            if (hasActiveSubscription)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Provider already has an active subscription.");

            var now = DateTime.UtcNow;

            request.Status = SubscriptionPaymentRequestStatus.Approved;
            request.ReviewedAt = now;
            request.ReviewedByAdminId = adminId;

            var subscription = new ProviderSubscription
            {
                ProviderProfileId = request.ProviderProfileId,
                SubscriptionPlanId = request.SubscriptionPlanId,
                SubscriptionPaymentRequestId = request.Id,
                StartsAt = now,
                EndsAt = now.AddDays(request.SubscriptionPlan.DurationInDays),
                CreatedAt = now
            };

            await _requestRepository.AddSubscriptionAsync(subscription);
            await _requestRepository.UpdateAsync(request);
            await _requestRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                request.ProviderProfile.UserId,
                "Subscription approved",
                "Your subscription payment request has been approved.",
                NotificationType.SubscriptionApproved,
                relatedSubscriptionPaymentRequestId: request.Id);
            // Later: create notification SubscriptionApproved

            var updatedRequest = await _requestRepository.GetByIdAsync(request.Id);

            return ServiceResult<SubscriptionPaymentRequest>.Success(updatedRequest!);
        }

        public async Task<ServiceResult<SubscriptionPaymentRequest>> RejectAsync(
            int requestId,
            int adminId,
            RejectSubscriptionPaymentRequestDto dto)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);

            if (request == null)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Subscription payment request not found.");

            if (request.Status != SubscriptionPaymentRequestStatus.Pending)
                return ServiceResult<SubscriptionPaymentRequest>.Failure("Only pending requests can be rejected.");

            request.Status = SubscriptionPaymentRequestStatus.Rejected;
            request.ReviewedAt = DateTime.UtcNow;
            request.ReviewedByAdminId = adminId;
            request.AdminNote = string.IsNullOrWhiteSpace(dto.AdminNote)
                ? null
                : dto.AdminNote.Trim();

            await _requestRepository.UpdateAsync(request);
            await _requestRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                request.ProviderProfile.UserId,
                "Subscription rejected",
                "Your subscription payment request has been rejected.",
                NotificationType.SubscriptionRejected,
                relatedSubscriptionPaymentRequestId: request.Id);
            // Later: create notification SubscriptionRejected

            var updatedRequest = await _requestRepository.GetByIdAsync(request.Id);

            return ServiceResult<SubscriptionPaymentRequest>.Success(updatedRequest!);
        }
    }
}
