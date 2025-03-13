using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class UpdateAnswerValidator : AbstractValidator<UpdateAnswerDto>
{
    public UpdateAnswerValidator()
    {
        RuleFor(e => e.Content).NotEmpty().WithMessage("Can not empty");
    }
}
