using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record AddUserToRoleCommand(int UserId, string Role) : ICommand<GenericResult<TextResponse>>;
