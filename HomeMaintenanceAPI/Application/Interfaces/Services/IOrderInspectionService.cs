using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.OrderInspection;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IOrderInspectionService
    {
        Task<ServiceResult<GenerateInspectionQrResponseDto>>
            GenerateInspectionQrAsync(int customerUserId, int orderId);

        Task<ServiceResult>
            ConfirmInspectionByQrAsync(
                int providerUserId,
                ConfirmInspectionByQrDto dto);
    }
}