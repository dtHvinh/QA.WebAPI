using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Utilities.Validators;

public class CreateReportValidator : AbstractValidator<CreateQuestionReportDto>
{
    public CreateReportValidator()
    {
        RuleFor(e => e.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(100).WithMessage("Description is too long");

        RuleFor(e => e.QuestionId)
            .NotEmpty().WithMessage("TargetId is required.");
    }
}
