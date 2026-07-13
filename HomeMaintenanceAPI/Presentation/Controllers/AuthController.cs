using System.Security.Claims;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Auth;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
        private readonly IValidator<ForgotPasswordDto> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
        private readonly IValidator<ChangePasswordDto> _changePasswordValidator;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator,
            IValidator<VerifyEmailDto> verifyEmailValidator,
            IValidator<ResendVerificationCodeDto> resendValidator,
            IValidator<RefreshTokenDto> refreshValidator,
            IValidator<ForgotPasswordDto> forgotPasswordValidator,
            IValidator<ResetPasswordDto> resetPasswordValidator,
            IValidator<ChangePasswordDto> changePasswordValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _verifyEmailValidator = verifyEmailValidator;
            _resendValidator = resendValidator;
            _refreshValidator = refreshValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _changePasswordValidator = changePasswordValidator;
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
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = GetCurrentUserId();

            var result = await _authService.GetCurrentUserAsync(userId);

            if (!result.Succeeded)
                return Unauthorized(result.Error);

            return Ok(result.Data);
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var validationResult = await _forgotPasswordValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var result = await _authService.ForgotPasswordAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("If the email exists, a password reset code has been sent.");
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Password has been reset successfully.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Password changed successfully. Please log in again.");
        }


        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

    }
}
