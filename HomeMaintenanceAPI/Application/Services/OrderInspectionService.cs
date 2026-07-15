using System.Security.Cryptography;
using System.Text;
using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.OrderInspection;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.Services
{
    public class OrderInspectionService : IOrderInspectionService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly ILogger<OrderInspectionService> _logger;

        public OrderInspectionService(
            IOrderRepository orderRepository,
            IProviderProfileRepository providerProfileRepository,
            ILogger<OrderInspectionService> logger)
        {
            _orderRepository = orderRepository;
            _providerProfileRepository = providerProfileRepository;
            _logger = logger;
        }
        public async Task<ServiceResult<GenerateInspectionQrResponseDto>>
                  GenerateInspectionQrAsync(
                      int customerUserId,
                      int orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult<GenerateInspectionQrResponseDto>
                    .Failure("Order not found.");
            }

            if (order.CustomerId != customerUserId)
            {
                return ServiceResult<GenerateInspectionQrResponseDto>
                    .Failure(
                        "You are not allowed to generate an inspection QR for this order.");
            }

            if (order.Status != OrderStatus.InspectionAccepted)
            {
                return ServiceResult<GenerateInspectionQrResponseDto>
                    .Failure(
                        "Inspection QR can only be generated after an offer is accepted for inspection.");
            }

            if (order.SelectedProviderProfileId == null)
            {
                return ServiceResult<GenerateInspectionQrResponseDto>
                    .Failure(
                        "No provider is selected for this order.");
            }
            var token = GenerateSecureToken();
            var tokenHash = ComputeHash(token);
            var expiresAt = DateTime.UtcNow.AddMinutes(10);
            order.InspectionTokenHash = tokenHash;
            order.InspectionTokenExpiresAt = expiresAt;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
            _logger.LogInformation(
                "Inspection QR generated. " +
                "OrderId={OrderId}, " +
                "CustomerId={CustomerId}, " +
                "SelectedProviderProfileId={ProviderProfileId}, " +
                "ExpiresAt={ExpiresAt}",
                order.Id,
                order.CustomerId,
                order.SelectedProviderProfileId,
                expiresAt);
            return ServiceResult<GenerateInspectionQrResponseDto>.Success(new GenerateInspectionQrResponseDto
            {
                OrderId = order.Id,
                Token = token,
                ExpiresAt = expiresAt
            });
        }
        public async Task<ServiceResult> ConfirmInspectionByQrAsync(
          int providerUserId,
          ConfirmInspectionByQrDto dto)
        {
            if (dto.OrderId <= 0)
            {
                return ServiceResult.Failure("A valid order ID is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.Token))
            {
                return ServiceResult.Failure(
                    "Inspection QR token is required.");
            }
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(providerUserId);

            if (providerProfile == null)
            {
                return ServiceResult.Failure(
                    "Provider profile not found.");
            }
            var order = await _orderRepository.GetByIdWithDetailsAsync(dto.OrderId);
            if (order == null)
            {
                return ServiceResult.Failure("Order not found.");
            }

            if (order.Status != OrderStatus.InspectionAccepted)
            {
                return ServiceResult.Failure(
                    "Order is not waiting for inspection confirmation.");
            }
            if (order.SelectedProviderProfileId != providerProfile.Id)
            {
                _logger.LogWarning(
                   "Inspection QR confirmation blocked. " +
                   "OrderId={OrderId}, " +
                   "ProviderUserId={ProviderUserId}, " +
                   "ProviderProfileId={ProviderProfileId}, " +
                   "SelectedProviderProfileId={SelectedProviderProfileId}, " +
                   "Reason={Reason}",
                   order.Id,
                   providerUserId,
                   providerProfile.Id,
                   order.SelectedProviderProfileId,
                   "Provider is not selected for this order");

                return ServiceResult.Failure("You are not the selected provider for this order.");
            }
            if (string.IsNullOrWhiteSpace(order.InspectionTokenHash) ||
               order.InspectionTokenExpiresAt == null)
            {
                return ServiceResult.Failure("No active inspection QR was found.");

            }
            if (order.InspectionTokenExpiresAt <= DateTime.UtcNow)
            {
                return ServiceResult.Failure(
                    "Inspection QR has expired.");
            }
            var receivedTokenHash = ComputeHash(dto.Token.Trim());
            if (receivedTokenHash != order.InspectionTokenHash)
            {
                _logger.LogWarning(
                    "Inspection QR confirmation failed. " +
                    "OrderId={OrderId}, " +
                    "ProviderProfileId={ProviderProfileId}, " +
                    "Reason={Reason}",
                    order.Id,
                    providerProfile.Id,
                    "Invalid token");

                return ServiceResult.Failure(
                    "Invalid inspection QR.");
            }
            order.Status = OrderStatus.InspectionInProgress;
            order.UpdatedAt = DateTime.UtcNow;
            order.InspectionTokenHash = null;
            order.InspectionTokenExpiresAt = null;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
            _logger.LogInformation(
                "Inspection started by QR. " +
                "OrderId={OrderId}, " +
                "CustomerId={CustomerId}, " +
                "ProviderProfileId={ProviderProfileId}",
                order.Id,
                order.CustomerId,
                providerProfile.Id);

            return ServiceResult.Success();
        }

        private static string GenerateSecureToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        private static string ComputeHash(string value)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
