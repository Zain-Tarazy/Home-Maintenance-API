using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Images;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;

namespace HomeMaintenanceAPI.Application.Services
{
    public class UserProfileImageService : IUserProfileImageService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UserProfileImageService> _logger;

        public UserProfileImageService(
            IUserRepository userRepository,
            IFileStorageService fileStorageService,
            ILogger<UserProfileImageService> logger)
        {
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<ServiceResult<ImageUploadResultDto>> UploadProfileImageAsync(
            int userId,
            Stream fileStream,
            string originalFileName,
            string contentType,
            long fileSize)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return ServiceResult<ImageUploadResultDto>.Failure("User not found.");

            var oldImageUrl = user.ProfileImageUrl;

            string newImageUrl;

            try
            {
                newImageUrl = await _fileStorageService.SaveImageAsync(
                    fileStream,
                    originalFileName,
                    contentType,
                    fileSize,
                    "profile-images");
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult<ImageUploadResultDto>.Failure(ex.Message);
            }

            user.ProfileImageUrl = newImageUrl;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            _fileStorageService.DeleteFile(oldImageUrl);

            _logger.LogInformation(
                "User profile image updated. UserId={UserId}, ImageUrl={ImageUrl}",
                user.Id,
                newImageUrl);

            return ServiceResult<ImageUploadResultDto>.Success(new ImageUploadResultDto
            {
                ImageUrl = newImageUrl
            });
        }
    }
}
