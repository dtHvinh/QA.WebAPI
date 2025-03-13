using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class UpdateCollectionValidator : AbstractValidator<UpdateCollectionDto>
{
    public UpdateCollectionValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(e => e.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(50)
            .WithMessage("Name must be less than 50 characters.");
    }
}
