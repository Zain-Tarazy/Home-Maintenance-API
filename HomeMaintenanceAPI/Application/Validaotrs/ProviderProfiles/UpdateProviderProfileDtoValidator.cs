using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;

namespace HomeMaintenanceAPI.Application.Validaotrs.ProviderProfiles
{
    public class UpdateProviderProfileDtoValidator : AbstractValidator<UpdateProviderProfileDto>
    {
        public UpdateProviderProfileDtoValidator()
        {
            RuleFor(x => x.Bio)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));
        }
    }
}
