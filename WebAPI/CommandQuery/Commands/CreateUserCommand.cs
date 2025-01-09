using CQRS;
using WebAPI.Dto;
using WebAPI.Utilities;

namespace WebAPI.CommandQuery.Commands;

public class CreateUserCommand(RegisterDto dto) : ICommand<HandlerResult<AuthResponseDto>>
{
    public RegisterDto User { get; } = dto;
}
