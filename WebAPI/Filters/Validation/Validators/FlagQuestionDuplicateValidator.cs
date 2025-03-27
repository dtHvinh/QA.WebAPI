using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class FlagQuestionDuplicateValidator : AbstractValidator<FlagQuestionDuplicateDto>
{
    public FlagQuestionDuplicateValidator()
    {
        RuleFor(e => e.DuplicateUrl)
            .NotEmpty()
            .WithMessage("DuplicateUrl is required.");
    }
}
