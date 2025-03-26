using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class UpdateCommunityValidator
    : AbstractValidator<UpdateCommunityDto>
{
    public UpdateCommunityValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}
