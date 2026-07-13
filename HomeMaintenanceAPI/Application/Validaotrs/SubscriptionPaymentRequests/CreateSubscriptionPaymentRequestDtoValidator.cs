using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;

namespace HomeMaintenanceAPI.Application.Validaotrs.SubscriptionPaymentRequests
{
    public class CreateSubscriptionPaymentRequestDtoValidator
        : AbstractValidator<CreateSubscriptionPaymentRequestDto>
    {
        public CreateSubscriptionPaymentRequestDtoValidator()
        {
            RuleFor(x => x.SubscriptionPlanId)
                .GreaterThan(0)
                .WithMessage("Subscription plan is required.");

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ProofImageUrl)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.ProofImageUrl));

            RuleFor(x => x.ProofImageUrl)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
