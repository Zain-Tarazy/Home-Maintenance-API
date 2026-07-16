using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.OrderInspection;

namespace HomeMaintenanceAPI.Application.Validaotrs.OrderInspection
{
    public class ConfirmInspectionByQrDtoValidator
           : AbstractValidator<ConfirmInspectionByQrDto>
    {
        public ConfirmInspectionByQrDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("A valid order ID is required.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Inspection QR token is required.")
                .MaximumLength(200)
                .WithMessage("Inspection QR token is invalid.");
        }
    }
}
