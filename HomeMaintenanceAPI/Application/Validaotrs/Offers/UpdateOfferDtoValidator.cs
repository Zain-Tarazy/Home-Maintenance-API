using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Offers;
using HomeMaintenanceAPI.Application.Helpers;

namespace HomeMaintenanceAPI.Application.Validaotrs.Offers
{
    public class UpdateOfferDtoValidator : AbstractValidator<UpdateOfferDto>
    {
        public UpdateOfferDtoValidator()
        {
            RuleFor(x => x.InspectionPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Inspection price cannot be negative.");

            RuleFor(x => x.Note)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Note));

            RuleFor(x => x.ProviderLatitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Provider latitude must be between -90 and 90.");

            RuleFor(x => x.ProviderLongitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Provider longitude must be between -180 and 180.");

            RuleFor(x => x)
                .Must(x => LocationValidationHelper.IsWithinServiceArea(
                    x.ProviderLatitude,
                    x.ProviderLongitude))
                .WithMessage("Provider location must be within the supported service area.");
        }
    }
}
