using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Utilities.Validations;

public class TagDtoValidator : AbstractValidator<CreateTagDto>
{
    public TagDtoValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MinimumLength(1)
            .WithMessage("Name too short")
            .MaximumLength(50)
            .WithMessage("Name too long");

        RuleFor(e => e.Description).NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Description too long");
    }
}
