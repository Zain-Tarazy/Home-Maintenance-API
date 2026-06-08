using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;

namespace HomeMaintenanceAPI.Application.Validaotrs.SubscriptionPlans
{
    public class CreateSubscriptionPlanDtoValidator : AbstractValidator<CreateSubscriptionPlanDto>
    {
        public CreateSubscriptionPlanDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DurationInDays)
                .GreaterThan(0);
        }
    }
}
