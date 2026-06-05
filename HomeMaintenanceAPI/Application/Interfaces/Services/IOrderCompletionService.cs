using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.OrderCompletion;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IOrderCompletionService
    {
        Task<ServiceResult<GenerateCompletionQrResponseDto>> GenerateCompletionQrAsync(
            int userId,
            int orderId);

        Task<ServiceResult> CompleteByQrAsync(
            int userId,
            CompleteOrderByQrDto dto);
    }
}
