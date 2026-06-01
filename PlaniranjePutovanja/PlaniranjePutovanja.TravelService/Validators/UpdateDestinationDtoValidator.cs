using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Validators
{
    public sealed class UpdateDestinationDtoValidator : AbstractValidator<UpdateDestinationDto>
    {
        public UpdateDestinationDtoValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Destination name is required.")
                .MaximumLength(200).WithMessage("Destination name must not exceed 200 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90m, 90m)
                .When(x => x.Latitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m)
            .When(x => x.Longitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.DaysSpent)
                .GreaterThan(0).WithMessage("DaysSpent must be greater than zero.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
        }
    }
}
