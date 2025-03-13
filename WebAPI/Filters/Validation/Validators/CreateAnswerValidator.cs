using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class CreateAnswerValidator : AbstractValidator<CreateAnswerDto>
{
    public CreateAnswerValidator()
    {
        RuleFor(e => e.Content)
            .NotEmpty().WithMessage("Content can not empty");
    }
}
