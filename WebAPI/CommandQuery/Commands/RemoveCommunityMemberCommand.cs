using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record RemoveCommunityMemberCommand(int CommunityId, int UserId)
    : ICommand<GenericResult<TextResponse>>;
