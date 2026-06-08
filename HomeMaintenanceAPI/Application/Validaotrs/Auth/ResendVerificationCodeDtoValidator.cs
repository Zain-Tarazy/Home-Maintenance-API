using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Auth;

namespace HomeMaintenanceAPI.Application.Validaotrs.Auth
{
    public class ResendVerificationCodeDtoValidator:AbstractValidator<ResendVerificationCodeDto>
    {
        public ResendVerificationCodeDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);
        }
    }
}
