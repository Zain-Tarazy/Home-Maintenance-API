using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;

namespace HomeMaintenanceAPI.Application.Validaotrs.ProviderProfiles
{
    public class CreateProviderProfileDtoValidator : AbstractValidator<CreateProviderProfileDto>
    {
        public CreateProviderProfileDtoValidator()
        {
            RuleFor(x => x.SpecializationId)
                .GreaterThan(0)
                .WithMessage("Specialization is required.");

            RuleFor(x => x.Bio)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));
        }
    }
}
