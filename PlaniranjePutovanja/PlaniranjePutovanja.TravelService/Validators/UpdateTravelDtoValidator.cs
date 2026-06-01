using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Validators
{
    public sealed class UpdateTravelDtoValidator : AbstractValidator<UpdateTravelDto>
    {
        public UpdateTravelDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Travel name is required.")
                .MaximumLength(200).WithMessage("Travel name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be greater than or equal to start date.");

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).WithMessage("Budget cannot be negative.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
        }
    }
}
