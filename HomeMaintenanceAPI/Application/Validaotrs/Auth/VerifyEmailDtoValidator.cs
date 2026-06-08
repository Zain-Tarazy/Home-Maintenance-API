using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Auth;

namespace HomeMaintenanceAPI.Application.Validaotrs.Auth
{
    public class VerifyEmailDtoValidator:AbstractValidator<VerifyEmailDto>
    {
        public VerifyEmailDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.Code)
                .NotEmpty()
                .Matches(@"^\d{6}$")
                .WithMessage("Verification code must be 6 digits.");
        }
    }
}
