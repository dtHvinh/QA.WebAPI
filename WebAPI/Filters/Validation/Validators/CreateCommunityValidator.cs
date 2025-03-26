using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class CreateCommunityValidator : AbstractValidator<CreateCommunityDto>
{
    public CreateCommunityValidator()
    {
        RuleFor(e => e.Name)
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Name must less than 50 chars long");

        RuleFor(e => e.IconImage)
            .NotNull().WithMessage("Icon image is required");
    }
}
