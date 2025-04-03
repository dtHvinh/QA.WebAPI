using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateInvitationLinkHandler(
    ICacheService cacheService,
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext)
    : ICommandHandler<CreateInvitationLinkCommand, GenericResult<InvitationLinkResponse>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<InvitationLinkResponse>> Handle(CreateInvitationLinkCommand request, CancellationToken cancellationToken)
    {
        if (!await _communityRepository.IsOwner(_authenticationContext.UserId, request.CommunityId, cancellationToken))
        {
            return GenericResult<InvitationLinkResponse>.Failure("You are not the owner of this community.");
        }

        var invitationLink = Guid.NewGuid().ToString()[..18];

        await _cacheService.SetInvitationLink(invitationLink, request.CommunityId, cancellationToken);

        return GenericResult<InvitationLinkResponse>.Success(new InvitationLinkResponse(invitationLink));
    }
}
