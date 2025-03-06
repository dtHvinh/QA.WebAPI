using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Utilities.Validators;

public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionDto>
{
    public UpdateQuestionValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title too long");

        RuleFor(e => e.Comment)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title too long");

        RuleFor(e => e.Tags)
            .NotEmpty().WithMessage("Tags is required.")
            .Must(e => e.Count <= 5).WithMessage("Tags limit is 5");
    }
}
