using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Auth;

namespace HomeMaintenanceAPI.Application.Validaotrs.Auth
{
    public class RegisterDtoValidator:AbstractValidator<RegisterDto>
    {
        private static readonly string EmailRegex =
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .MaximumLength(254).WithMessage("Email must not exceed 254 characters.")
                .Matches(EmailRegex).WithMessage("Please enter a valid email address format.")
                .Must(email => !email.Contains("..")).WithMessage("Email cannot contain consecutive dots.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^09\d{8}$")
                .WithMessage("Phone number must be 10 digits and start with 09.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters.")
                .MaximumLength(100);
        }
    }
}
