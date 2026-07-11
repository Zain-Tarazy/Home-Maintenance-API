using System.Security.Cryptography;
using System.Text;
using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.OrderCompletion;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Services
{
    public class OrderCompletionService : IOrderCompletionService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<OrderCompletionService> _logger;

        public OrderCompletionService(
            IOrderRepository orderRepository,
            IProviderProfileRepository providerProfileRepository,
            INotificationService notificationService,
            ILogger<OrderCompletionService> logger)
        {
            _orderRepository = orderRepository;
            _providerProfileRepository = providerProfileRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<ServiceResult<GenerateCompletionQrResponseDto>> GenerateCompletionQrAsync(
            int userId,
            int orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null)
                return ServiceResult<GenerateCompletionQrResponseDto>.Failure("Order not found.");

            if (order.CustomerId != userId)
                return ServiceResult<GenerateCompletionQrResponseDto>.Failure("You are not allowed to generate QR for this order.");

            if (order.Status != OrderStatus.InProgress &&
                order.Status != OrderStatus.CompletionPending)
            {
                return ServiceResult<GenerateCompletionQrResponseDto>.Failure(
                    "QR can only be generated for orders in progress.");
            }

            if (order.SelectedProviderProfileId == null)
                return ServiceResult<GenerateCompletionQrResponseDto>.Failure("No provider is selected for this order.");

            var token = GenerateSecureToken();
            var tokenHash = ComputeHash(token);
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            order.CompletionTokenHash = tokenHash;
            order.CompletionTokenExpiresAt = expiresAt;
            order.Status = OrderStatus.CompletionPending;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                order.SelectedProviderProfile!.UserId,
                "Order completion pending",
                "The customer generated a completion QR for the order.",
                NotificationType.OrderCompletionPending,
                relatedOrderId: order.Id);
            
            _logger.LogInformation(
                "Completion QR generated. OrderId={OrderId}, CustomerId={CustomerId}, SelectedProviderProfileId={ProviderProfileId}, ExpiresAt={ExpiresAt}",
                order.Id,
                order.CustomerId,
                order.SelectedProviderProfileId,
                order.CompletionTokenExpiresAt);

            return ServiceResult<GenerateCompletionQrResponseDto>.Success(
                new GenerateCompletionQrResponseDto
                {
                    OrderId = order.Id,
                    Token = token,
                    ExpiresAt = expiresAt
                });
        }

        public async Task<ServiceResult> CompleteByQrAsync(
            int userId,
            CompleteOrderByQrDto dto)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult.Failure("Provider profile not found.");

            var order = await _orderRepository.GetByIdWithDetailsAsync(dto.OrderId);

            if (order == null)
                return ServiceResult.Failure("Order not found.");

            if (order.Status != OrderStatus.CompletionPending)
                return ServiceResult.Failure("Order is not waiting for QR completion.");

            if (order.SelectedProviderProfileId != providerProfile.Id)
            {
                _logger.LogWarning(
                    "QR completion blocked. OrderId={OrderId}, UserId={UserId}, ProviderProfileId={ProviderProfileId}, Reason={Reason}",
                    dto.OrderId,
                    order.SelectedProviderProfileId,
                    providerProfile?.Id,
                    "Provider is not selected for this order");
                return ServiceResult.Failure("You are not the selected provider for this order.");
            }
            if (string.IsNullOrWhiteSpace(order.CompletionTokenHash) ||
                order.CompletionTokenExpiresAt == null)
            {
                return ServiceResult.Failure("No active completion QR found.");
            }

            if (order.CompletionTokenExpiresAt <= DateTime.UtcNow)
                return ServiceResult.Failure("Completion QR has expired.");

            var receivedTokenHash = ComputeHash(dto.Token);

            if (receivedTokenHash != order.CompletionTokenHash)
                return ServiceResult.Failure("Invalid completion QR.");

            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            order.CompletionTokenHash = null;
            order.CompletionTokenExpiresAt = null;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            await _notificationService.CreateAndSendAsync(
                order.CustomerId,
                "Order completed",
                "Your order has been completed successfully.",
                NotificationType.OrderCompleted,
                relatedOrderId: order.Id);

            _logger.LogInformation(
                "Order completed by QR. OrderId={OrderId}, ProviderProfileId={ProviderProfileId}",
                order.Id,
                providerProfile.Id);

            return ServiceResult.Success();

            // Later: notify customer OrderCompleted
        }

        private string GenerateSecureToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        private string ComputeHash(string value)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
