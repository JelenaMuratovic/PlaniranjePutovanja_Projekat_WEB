using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Validators
{
    public sealed class CreateActivityDtoValidator : AbstractValidator<CreateActivityDto>
    {
        public CreateActivityDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Activity name is required.")
                .MaximumLength(200).WithMessage("Activity name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.ActivityDate)
                .NotEmpty().WithMessage("Activity date is required.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Activity status is not valid.");
        }
    }
}
