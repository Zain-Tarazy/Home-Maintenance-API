using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Auth;

namespace HomeMaintenanceAPI.Application.Validaotrs.Auth
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.Code)
                .NotEmpty()
                .Length(6)
                .WithMessage("Reset code must be 6 digits.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}
