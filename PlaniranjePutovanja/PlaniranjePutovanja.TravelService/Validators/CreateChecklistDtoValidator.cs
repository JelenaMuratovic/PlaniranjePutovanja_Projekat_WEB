using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Validators
{
    public sealed class CreateChecklistDtoValidator : AbstractValidator<CreateChecklistDto>
    {
        public CreateChecklistDtoValidator()
        {
            RuleFor(x => x.Item)
                .NotEmpty().WithMessage("Checklist item is required.")
                .MaximumLength(500).WithMessage("Checklist item must not exceed 500 characters.");
        }
    }
}
