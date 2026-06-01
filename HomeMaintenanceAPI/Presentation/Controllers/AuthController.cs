using HomeMaintenanceAPI.Application.DTOs.Auth;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;


namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Registration successful. Verification code sent to your email.");
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
        {
            var result = await _authService.VerifyEmailAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Email verified successfully.");
        }

        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode(ResendVerificationCodeDto dto)
        {
            var result = await _authService.ResendVerificationCodeAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("A new verification code has been sent to your email.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(new AuthResponseDto
            {
                AccessToken = result.Data!.AccessToken,
                RefreshToken = result.Data.RefreshToken
            });
        }

    }
}
