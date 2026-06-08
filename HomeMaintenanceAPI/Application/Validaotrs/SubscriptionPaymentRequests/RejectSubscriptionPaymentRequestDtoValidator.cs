using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;

namespace HomeMaintenanceAPI.Application.Validaotrs.SubscriptionPaymentRequests
{
    public class RejectSubscriptionPaymentRequestDtoValidator
        : AbstractValidator<RejectSubscriptionPaymentRequestDto>
    {
        public RejectSubscriptionPaymentRequestDtoValidator()
        {
            RuleFor(x => x.AdminNote)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.AdminNote));
        }
    }
}
