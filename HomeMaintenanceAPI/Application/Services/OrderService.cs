using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Orders;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;


namespace HomeMaintenanceAPI.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ISpecializationRepository specializationRepository,
            IProviderProfileRepository providerProfileRepository,
            INotificationService notificationService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _specializationRepository = specializationRepository;
            _providerProfileRepository = providerProfileRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<ServiceResult<Order>> CreateAsync(int customerId, CreateOrderDto dto)
        {
            var validationResult = ValidateOrderInput(dto.Description, dto.Latitude, dto.Longitude);

            if (!validationResult.Succeeded)
                return ServiceResult<Order>.Failure(validationResult.Error!);

            var specialization = await _specializationRepository.GetByIdAsync(dto.SpecializationId);

            if (specialization == null)
                return ServiceResult<Order>.Failure("Specialization not found.");

            if (!specialization.IsActive)
            {
                _logger.LogWarning(
                    "Order creation failed. CustomerId={CustomerId}, SpecializationId={SpecializationId}, Reason={Reason}",
                    customerId,
                    dto.SpecializationId,
                    "Specialization inactive");
                return ServiceResult<Order>.Failure("Specialization is not active.");
            }
            var order = new Order
            {
                CustomerId = customerId,
                SpecializationId = dto.SpecializationId,
                Description = dto.Description.Trim(),
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                AddressText = string.IsNullOrWhiteSpace(dto.AddressText)
                    ? null
                    : dto.AddressText.Trim(),
                Status = OrderStatus.WaitingForOffers,
                SelectedProviderProfileId = null
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            var createdOrder = await _orderRepository.GetByIdWithDetailsAsync(order.Id);


            _logger.LogInformation(
                "Order created. OrderId={OrderId}, CustomerId={CustomerId}, SpecializationId={SpecializationId}",
                order.Id,
                order.CustomerId,
                order.SpecializationId);

            return ServiceResult<Order>.Success(createdOrder!);
        }

        public async Task<ServiceResult<PagedResult<Order>>> GetMineAsync(
            int customerId,
            PaginationParams paginationParams)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(
                customerId,
                paginationParams);

            return ServiceResult<PagedResult<Order>>.Success(orders);
        }

        public async Task<ServiceResult<Order>> GetByIdAsync(int currentUserId, int orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null)
                return ServiceResult<Order>.Failure("Order not found.");

            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(currentUserId);

            var isCustomer = order.CustomerId == currentUserId;

            var isSelectedProvider =
                providerProfile != null &&
                order.SelectedProviderProfileId == providerProfile.Id;

            var isMatchingProviderForAvailableOrder =
                providerProfile != null &&
                order.Status == OrderStatus.WaitingForOffers &&
                order.SpecializationId == providerProfile.SpecializationId &&
                order.CustomerId != currentUserId;

            if (!isCustomer && !isSelectedProvider && !isMatchingProviderForAvailableOrder)
                return ServiceResult<Order>.Failure("You are not allowed to view this order.");

            return ServiceResult<Order>.Success(order);
        }

        public async Task<ServiceResult<Order>> UpdateAsync(int customerId, int orderId, UpdateOrderDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return ServiceResult<Order>.Failure("Order not found.");

            if (order.CustomerId != customerId)
                return ServiceResult<Order>.Failure("You are not allowed to update this order.");

            if (order.Status != OrderStatus.WaitingForOffers)
                return ServiceResult<Order>.Failure("Only orders waiting for offers can be updated.");

            var hasOffers = await _orderRepository.HasOffersAsync(order.Id);

            if (hasOffers)
            {
                _logger.LogWarning(
                    "Order update blocked. OrderId={OrderId}, CustomerId={CustomerId}, Reason={Reason}",
                    orderId,
                    customerId,
                    "Order already has offers");
                return ServiceResult<Order>.Failure(
                    "Cannot update order after offers have been submitted. Cancel it and create a new order.");
            }

            var validationResult = ValidateOrderInput(dto.Description, dto.Latitude, dto.Longitude);

            if (!validationResult.Succeeded)
                return ServiceResult<Order>.Failure(validationResult.Error!);

            var specialization = await _specializationRepository.GetByIdAsync(dto.SpecializationId);

            if (specialization == null)
                return ServiceResult<Order>.Failure("Specialization not found.");

            if (!specialization.IsActive)
                return ServiceResult<Order>.Failure("Specialization is not active.");

            order.SpecializationId = dto.SpecializationId;
            order.Description = dto.Description.Trim();
            order.Latitude = dto.Latitude;
            order.Longitude = dto.Longitude;
            order.AddressText = string.IsNullOrWhiteSpace(dto.AddressText)
                ? null
                : dto.AddressText.Trim();
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            var updatedOrder = await _orderRepository.GetByIdWithDetailsAsync(order.Id);


            _logger.LogInformation(
                "Order updated. OrderId={OrderId}, CustomerId={CustomerId}",
                order.Id,
                customerId);

            return ServiceResult<Order>.Success(updatedOrder!);
        }

        public async Task<ServiceResult> CancelAsync(int customerId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return ServiceResult.Failure("Order not found.");

            if (order.CustomerId != customerId)
                return ServiceResult.Failure("You are not allowed to cancel this order.");

            if (order.Status != OrderStatus.WaitingForOffers)
                return ServiceResult.Failure("Only orders waiting for offers can be cancelled.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            foreach (var offer in order.Offers.Where(o => o.Status == OfferStatus.Pending))
            {
                offer.Status = OfferStatus.CancelledDueToOrderCancellation;
                offer.UpdatedAt = DateTime.UtcNow;
            }

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            var providersToNotify = order.Offers
                .Where(o => o.ProviderProfile != null)
                .Select(o => o.ProviderProfile.UserId)
                .Distinct()
                .ToList();

            foreach (var providerUserId in providersToNotify)
            {
                await _notificationService.CreateAndSendAsync(
                    providerUserId,
                    "Order cancelled",
                    "The customer cancelled an order you submitted an offer for.",
                    NotificationType.OrderCancelled,
                    relatedOrderId: order.Id);
            }


            _logger.LogInformation(
                "Order cancelled. OrderId={OrderId}, CustomerId={CustomerId}",
                order.Id,
                customerId);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<PagedResult<Order>>> GetAvailableForProviderAsync(
             int providerUserId,
             PaginationParams paginationParams)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(providerUserId);

            if (providerProfile == null)
                return ServiceResult<PagedResult<Order>>.Failure("Provider profile not found.");

            var currentSpecialization = await _specializationRepository
                .GetByIdAsync(providerProfile.SpecializationId);

            if (currentSpecialization == null)
                return ServiceResult<PagedResult<Order>>.Failure("Specialization not found.");

            if (!currentSpecialization.IsActive)
                return ServiceResult<PagedResult<Order>>.Failure(
                "Your specialization is currently inactive. You cannot receive new orders until it is reactivated by the admin.");

            var orders = await _orderRepository.GetAvailableForProviderAsync(
                providerUserId,
                providerProfile.SpecializationId,
                paginationParams);

            return ServiceResult<PagedResult<Order>>.Success(orders);
        }

        public async Task<ServiceResult<PagedResult<Order>>> GetAssignedForProviderAsync(
            int providerUserId,
            PaginationParams paginationParams)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(providerUserId);

            if (providerProfile == null)
                return ServiceResult<PagedResult<Order>>.Failure("Provider profile not found.");

            var orders = await _orderRepository.GetAssignedForProviderAsync(
                providerProfile.Id,
                paginationParams);

            return ServiceResult<PagedResult<Order>>.Success(orders);
        }

        private ServiceResult ValidateOrderInput(string description, decimal latitude, decimal longitude)
        {
            if (string.IsNullOrWhiteSpace(description))
                return ServiceResult.Failure("Description is required.");

            if (latitude < -90 || latitude > 90)
                return ServiceResult.Failure("Latitude must be between -90 and 90.");

            if (longitude < -180 || longitude > 180)
                return ServiceResult.Failure("Longitude must be between -180 and 180.");

            return ServiceResult.Success();
        }
    }
}
