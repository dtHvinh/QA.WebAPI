using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class CreateCommunityChatRoomValidator : AbstractValidator<CreateCommunityChatRoomDto>
{
    public CreateCommunityChatRoomValidator()
    {
        RuleFor(expression: x => x.CommunityId)
            .NotEmpty()
            .WithMessage("CommunityId is required");

        RuleFor(expression: x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name must be less than 50 characters");
    }
}
