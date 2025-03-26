using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CommunityModRoleCommand(int UserId, int CommunityId, bool IsGranting) : ICommand<GenericResult<TextResponse>>;
