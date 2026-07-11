using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Auth;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace HomeMaintenanceAPI.Application.Services

{
    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IEmailService emailService,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ServiceResult> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            var phoneNumber = dto.PhoneNumber.Trim();

            if (await _userRepository.EmailExistsAsync(email))
            {
                _logger.LogWarning("Registration failed. Email={Email} is already registered.", email);
                return ServiceResult.Failure("Email is already registered.");
            }

            if (await _userRepository.PhoneNumberExistsAsync(phoneNumber))
            {
                _logger.LogWarning("Registration failed. PhoneNumber={PhoneNumber} is already registered.", phoneNumber);
                return ServiceResult.Failure("Phone number is already registered.");
            }

            var salt = GenerateSalt();
            var passwordHash = ComputeHash(dto.Password, salt);

            var otp = GenerateOtp();
            var otpHash = ComputeHash(otp, salt);

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                PhoneNumber = phoneNumber,
                PasswordSalt = salt,
                PasswordHash = passwordHash,
                Role = UserRole.User,
                IsEmailVerified = false,
                EmailVerificationCodeHash = otpHash,
                EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Email verification code",
                $"Your verification code is: {otp}");


            _logger.LogInformation(
                "User registered successfully. UserId={UserId}, Email={Email}",
                user.Id,
                user.Email);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> VerifyEmailAsync(VerifyEmailDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("Email verification failed. Email={Email}, Reason={Reason}", email, "User not found");
                return ServiceResult.Failure("Invalid email or verification code.");
            }

            if (user.IsEmailVerified)
            {
                _logger.LogInformation("Email is already verified. UserId={UserId}, Email={Email}", user.Id, user.Email);
                return ServiceResult.Success();
            }

            if (user.EmailVerificationCodeHash == null ||
                user.EmailVerificationCodeExpiresAt == null)
            {
                _logger.LogWarning("Email verification failed. UserId={UserId}, Email={Email}, Reason={Reason}", user.Id, user.Email, "No verification code found");
                return ServiceResult.Failure("No verification code found. Please request a new code.");
            }

            if (user.EmailVerificationCodeExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Email verification failed. UserId={UserId}, Email={Email}, Reason={Reason}", user.Id, user.Email, "Verification code has expired");
                return ServiceResult.Failure("Verification code has expired.");
            }

            var receivedCodeHash = ComputeHash(dto.Code, user.PasswordSalt);

            if (receivedCodeHash != user.EmailVerificationCodeHash)
            {
                _logger.LogWarning("Email verification failed. UserId={UserId}, Email={Email}, Reason={Reason}", user.Id, user.Email, "Invalid verification code");
                return ServiceResult.Failure("Invalid email or verification code.");
            }
            user.IsEmailVerified = true;
            user.EmailVerificationCodeHash = null;
            user.EmailVerificationCodeExpiresAt = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Email verified successfully. UserId={UserId}, Email={Email}",
                user.Id,
                user.Email);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ResendVerificationCodeAsync(ResendVerificationCodeDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return ServiceResult.Failure("User not found.");

            if (user.IsEmailVerified)
                return ServiceResult.Failure("Email is already verified.");

            var otp = GenerateOtp();
            var otpHash = ComputeHash(otp, user.PasswordSalt);

            user.EmailVerificationCodeHash = otpHash;
            user.EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            await _emailService.SendEmailAsync(
            user.Email,
            "Home Maintenance App - Email Verification",
            $"Hello {user.FullName},\n\nYour verification code is: {otp}\n\nThis code expires in 10 minutes.\n\nIf you did not request this, ignore this email."
            );

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<AuthResult>> LoginAsync(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning(
                    "Login failed. Email={Email}, Reason={Reason}",
                    dto.Email,
                    "User not found");
                return ServiceResult<AuthResult>.Failure("Invalid email or password.");
            }
            if (!user.IsEmailVerified)
            {
                _logger.LogWarning(
                    "Login failed. UserId={UserId}, Email={Email}, Reason={Reason}",
                    user.Id,
                    user.Email,
                    "Email not verified");
                return ServiceResult<AuthResult>.Failure("Please verify your email first.");
            }
            var passwordHash = ComputeHash(dto.Password, user.PasswordSalt);

            if (passwordHash != user.PasswordHash)
            {
                _logger.LogWarning(
                    "Login failed. UserId={UserId}, Email={Email}, Reason={Reason}",
                    user.Id,
                    user.Email,
                    "Invalid password");
                return ServiceResult<AuthResult>.Failure("Invalid email or password.");
            }
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(90);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var authResult = new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            _logger.LogInformation(
                "User logged in successfully. UserId={UserId}, Email={Email}",
                user.Id,
                user.Email);
            return ServiceResult<AuthResult>.Success(authResult);
        }


        public async Task<ServiceResult<AuthResult>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);

            if (user == null)
                return ServiceResult<AuthResult>.Failure("Invalid refresh token.");

            if (user.RefreshTokenExpiryTime == null ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ServiceResult<AuthResult>.Failure("Refresh token has expired.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult<AuthResult>.Success(new AuthResult
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        public async Task<ServiceResult> LogoutAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return ServiceResult.Failure("User not found.");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<CurrentUserDto>> GetCurrentUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithProviderDetailsAsync(userId);

            if (user == null)
                return ServiceResult<CurrentUserDto>.Failure("User not found.");

            var now = DateTime.UtcNow;

            var providerProfile = user.ProviderProfile;

            var activeSubscription = providerProfile?.Subscriptions
                .Where(s => s.StartsAt <= now && s.EndsAt > now)
                .OrderByDescending(s => s.EndsAt)
                .FirstOrDefault();

            var dto = new CurrentUserDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),

                HasProviderProfile = providerProfile != null,
                ProviderProfileId = providerProfile?.Id,

                SpecializationId = providerProfile?.SpecializationId,
                SpecializationName = providerProfile?.Specialization?.Name,
                SpecializationIsActive = providerProfile?.Specialization?.IsActive,

                HasActiveSubscription = activeSubscription != null,
                ActiveSubscriptionEndsAt = activeSubscription?.EndsAt
            };

            return ServiceResult<CurrentUserDto>.Success(dto);
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            // Security rule:
            // Do not reveal whether this email exists or not.
            if (user == null)
            {
                return ServiceResult.Success();
            }

            var resetCode = GenerateOtp();

            user.PasswordResetCodeHash = ComputeHash(resetCode, user.PasswordSalt);
            user.PasswordResetCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Password Reset Code",
                $"Your password reset code is: {resetCode}. It expires in 10 minutes.");

            _logger.LogInformation(
                "Password reset code sent. UserId={UserId}, Email={Email}",
                user.Id,
                user.Email);

            return ServiceResult.Success();
        }


        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
                return ServiceResult.Failure("Invalid email or reset code.");

            if (string.IsNullOrWhiteSpace(user.PasswordResetCodeHash) ||
                user.PasswordResetCodeExpiresAt == null)
            {
                return ServiceResult.Failure("Invalid email or reset code.");
            }

            if (user.PasswordResetCodeExpiresAt < DateTime.UtcNow)
            {
                return ServiceResult.Failure("Reset code has expired.");
            }

            var submittedCodeHash = ComputeHash(dto.Code, user.PasswordSalt);

            if (submittedCodeHash != user.PasswordResetCodeHash)
            {
                return ServiceResult.Failure("Invalid email or reset code.");
            }

            var newSalt = GenerateSalt();
            var newPasswordHash = ComputeHash(dto.NewPassword, newSalt);

            user.PasswordSalt = newSalt;
            user.PasswordHash = newPasswordHash;

            user.PasswordResetCodeHash = null;
            user.PasswordResetCodeExpiresAt = null;

            // Security: invalidate refresh token after password reset.
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Password reset successfully. UserId={UserId}, Email={Email}",
                user.Id,
                user.Email);

            return ServiceResult.Success();
        }






        //------------------
        private string GenerateSalt()
        {
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(saltBytes);
        }

        private string GenerateOtp()
        {
            var number = RandomNumberGenerator.GetInt32(100000, 1000000);
            return number.ToString();
        }

        private string ComputeHash(string value, string salt)
        {
            using var sha256 = SHA256.Create();

            var combined = value + salt;
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
