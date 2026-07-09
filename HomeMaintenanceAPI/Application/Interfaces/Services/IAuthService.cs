using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Auth;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResult> RegisterAsync(RegisterDto dto);
        Task<ServiceResult> VerifyEmailAsync(VerifyEmailDto dto);
        Task<ServiceResult> ResendVerificationCodeAsync(ResendVerificationCodeDto dto);
        Task<ServiceResult<AuthResult>> LoginAsync(LoginDto dto);
        Task<ServiceResult<AuthResult>> RefreshTokenAsync(RefreshTokenDto dto);
        Task<ServiceResult<CurrentUserDto>> GetCurrentUserAsync(int userId);
        Task<ServiceResult> LogoutAsync(int userId);
    }
}
