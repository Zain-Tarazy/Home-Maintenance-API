using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Ratings;

namespace HomeMaintenanceAPI.Application.Validaotrs.Ratings
{
    public class CreateRatingDtoValidator : AbstractValidator<CreateRatingDto>
    {
        public CreateRatingDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("Order is required.");

            RuleFor(x => x.Value)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating value must be between 1 and 5.");
        }
    }
}
