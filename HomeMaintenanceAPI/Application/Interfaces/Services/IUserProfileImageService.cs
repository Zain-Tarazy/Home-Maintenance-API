using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Images;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IUserProfileImageService
    {
        Task<ServiceResult<ImageUploadResultDto>> UploadProfileImageAsync(
            int userId,
            Stream fileStream,
            string originalFileName,
            string contentType,
            long fileSize);
    }
}
