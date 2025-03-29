using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class ChatRequestValidator
    : AbstractValidator<ChatRequestDto>
{
    public ChatRequestValidator()
    {
        RuleFor(e => e.ChatRoomId)
            .NotEmpty()
            .WithMessage("Chat room is required");

        // User may just send a file 
        //RuleFor(e => e.Message)
        //    .NotEmpty()
        //    .WithMessage("Message is required");
    }
}
