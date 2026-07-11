using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Auth;

namespace HomeMaintenanceAPI.Application.Validaotrs.Auth
{
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);
        }
    }
}
