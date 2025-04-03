using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record JoinCommunityViaInvitationLinkCommand(string InvitationLink) : ICommand<GenericResult<TextResponse>>;
