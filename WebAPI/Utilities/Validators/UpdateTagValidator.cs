using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Utilities.Validators;

public class UpdateTagValidator : AbstractValidator<UpdateTagDto>
{
    public UpdateTagValidator()
    {
        RuleFor(e => e.Id).NotEmpty().WithMessage("Id is required");

        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name is too long");

        RuleFor(e => e.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description is too long");
    }
}
