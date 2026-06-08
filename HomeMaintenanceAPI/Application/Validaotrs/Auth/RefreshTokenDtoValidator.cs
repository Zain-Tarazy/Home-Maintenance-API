using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Auth;
namespace HomeMaintenanceAPI.Application.Validaotrs.Auth
{
    public class RefreshTokenDtoValidator:AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.");
        }
    }
}
