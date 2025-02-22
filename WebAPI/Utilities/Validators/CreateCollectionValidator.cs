using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Utilities.Validators;

public class CreateCollectionValidator : AbstractValidator<CreateCollectionDto>
{
    public CreateCollectionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}
