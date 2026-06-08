using System.Security.Claims;
using HomeMaintenanceAPI.Application.DTOs.Auth;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;


namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<VerifyEmailDto> _verifyEmailValidator;
        private readonly IValidator<ResendVerificationCodeDto> _resendValidator;
        private readonly IValidator<RefreshTokenDto> _refreshValidator;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator,
            IValidator<VerifyEmailDto> verifyEmailValidator,
            IValidator<ResendVerificationCodeDto> resendValidator,
            IValidator<RefreshTokenDto> refreshValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _verifyEmailValidator = verifyEmailValidator;
            _resendValidator = resendValidator;
            _refreshValidator = refreshValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var validation = await _registerValidator.ValidateAsync(dto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));


            var result = await _authService.RegisterAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Registration successful. Verification code sent to your email.");
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
        {

            var validation  = await _verifyEmailValidator.ValidateAsync(dto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e=>e.ErrorMessage));


            var result = await _authService.VerifyEmailAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Email verified successfully.");
        }

        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode(ResendVerificationCodeDto dto)
        {
            var validation = await _resendValidator.ValidateAsync(dto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));


            var result = await _authService.ResendVerificationCodeAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("A new verification code has been sent to your email.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var validation = await _loginValidator.ValidateAsync(dto);
             
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));


            var result = await _authService.LoginAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(new AuthResponseDto
            {
                AccessToken = result.Data!.AccessToken,
                RefreshToken = result.Data.RefreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            var validation = await _refreshValidator.ValidateAsync(dto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));


            var result = await _authService.RefreshTokenAsync(dto);

            if (!result.Succeeded)
                return Unauthorized(result.Error);

            return Ok(new AuthResponseDto
            {
                AccessToken = result.Data!.AccessToken,
                RefreshToken = result.Data.RefreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _authService.LogoutAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Logged out successfully.");
        }

    }
}
