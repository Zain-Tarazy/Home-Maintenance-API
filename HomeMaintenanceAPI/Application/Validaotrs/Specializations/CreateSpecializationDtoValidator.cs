using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Specialization;

namespace HomeMaintenanceAPI.Application.Validaotrs.Specializations
{
    public class CreateSpecializationDtoValidator : AbstractValidator<CreateSpecializationDto>
    {
        public CreateSpecializationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
