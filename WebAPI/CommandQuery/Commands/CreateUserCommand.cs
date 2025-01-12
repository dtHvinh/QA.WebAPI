using CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateUserCommand(RegisterDto dto) : ICommand<OperationResult<AuthResponseDto>>
{
    public RegisterDto User { get; } = dto;
}
