using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Utilities.Validators;

public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentValidator()
    {
        RuleFor(e => e.Content)
            .NotEmpty().WithMessage("Can not send empty")
            .MaximumLength(2000).WithMessage("Comment too long");
    }
}
