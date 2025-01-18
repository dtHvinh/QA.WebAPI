using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response.AuthResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateUserCommand(RegisterDto dto) : ICommand<GenericResult<AuthResponse>>
{
    public RegisterDto User { get; } = dto;
}
