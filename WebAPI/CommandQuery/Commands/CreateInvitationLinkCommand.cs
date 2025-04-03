using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateInvitationLinkCommand(int CommunityId)
    : ICommand<GenericResult<InvitationLinkResponse>>;
