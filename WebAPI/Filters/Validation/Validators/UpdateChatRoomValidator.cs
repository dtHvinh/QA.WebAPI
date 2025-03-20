using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class UpdateChatRoomValidator
    : AbstractValidator<UpdateChatRoomDto>
{
    public UpdateChatRoomValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty()
            .WithMessage("Chat room id is required.");

        RuleFor(e => e.Name)
            .NotEmpty()
            .WithMessage("Chat room name is required.")
            .MaximumLength(50)
            .WithMessage("Chat room name must be less than 50 characters.");
    }
}
