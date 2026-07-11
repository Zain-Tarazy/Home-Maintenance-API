using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Ratings;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<RatingService> _logger;

        public RatingService(
            IRatingRepository ratingRepository,
            IOrderRepository orderRepository,
            IProviderProfileRepository providerProfileRepository,
            INotificationService notificationService,
            ILogger<RatingService> logger)
        {
            _ratingRepository = ratingRepository;
            _orderRepository = orderRepository;
            _providerProfileRepository = providerProfileRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<ServiceResult<Rating>> CreateAsync(int userId, CreateRatingDto dto)
        {
            if (dto.Value < 1 || dto.Value > 5)
                return ServiceResult<Rating>.Failure("Rating value must be between 1 and 5.");

            var order = await _orderRepository.GetByIdWithDetailsAsync(dto.OrderId);

            if (order == null)
                return ServiceResult<Rating>.Failure("Order not found.");

            if (order.CustomerId != userId)
                return ServiceResult<Rating>.Failure("You are not allowed to rate this order.");

            if (order.Status != OrderStatus.Completed)
                return ServiceResult<Rating>.Failure("Only completed orders can be rated.");

            if (order.SelectedProviderProfileId == null)
                return ServiceResult<Rating>.Failure("No provider was selected for this order.");

            if (await _ratingRepository.ExistsForOrderAsync(order.Id))
            {
                _logger.LogWarning(
                    "Rating creation blocked. OrderId={OrderId}, CustomerId={CustomerId}, Reason={Reason}",
                    dto.OrderId,
                    order.CustomerId,
                    "Order already rated");
                return ServiceResult<Rating>.Failure("This order has already been rated.");
            }
            var rating = new Rating
            {
                OrderId = order.Id,
                CustomerId = userId,
                ProviderProfileId = order.SelectedProviderProfileId.Value,
                Value = dto.Value,
                CreatedAt = DateTime.UtcNow
            };

            await _ratingRepository.AddAsync(rating);
            await _ratingRepository.SaveChangesAsync();

            var createdRating = await _ratingRepository.GetByIdWithDetailsAsync(rating.Id);

            await _notificationService.CreateAndSendAsync(
                 order.SelectedProviderProfile!.UserId,
                 "New rating received",
                 "A customer rated you after a completed order.",
                 NotificationType.RatingReceived,
                 relatedOrderId: order.Id);

            _logger.LogInformation(
                "Rating created. RatingId={RatingId}, OrderId={OrderId}, CustomerId={CustomerId}, ProviderProfileId={ProviderProfileId}, Value={Value}",
                rating.Id,
                rating.OrderId,
                rating.CustomerId,
                rating.ProviderProfileId,
                rating.Value);

            return ServiceResult<Rating>.Success(createdRating!);
        }

        public async Task<ServiceResult<ProviderRatingSummaryDto>> GetProviderSummaryAsync(int providerProfileId)
        {
            var providerProfile = await _providerProfileRepository.GetByIdAsync(providerProfileId);

            if (providerProfile == null)
                return ServiceResult<ProviderRatingSummaryDto>.Failure("Provider profile not found.");

            var ratings = await _ratingRepository.GetByProviderProfileIdAsync(providerProfileId);

            var ratingsCount = ratings.Count;

            var averageRating = ratingsCount == 0
                ? 0
                : ratings.Average(r => r.Value);

            var completedOrdersCount =
                await _ratingRepository.GetCompletedOrdersCountAsync(providerProfileId);

            var summary = new ProviderRatingSummaryDto
            {
                ProviderProfileId = providerProfileId,
                AverageRating = Math.Round(averageRating, 2),
                RatingsCount = ratingsCount,
                CompletedOrdersCount = completedOrdersCount
            };

            return ServiceResult<ProviderRatingSummaryDto>.Success(summary);
        }
    }
}
