using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetInvitationLinkHandler(
    ICacheService cacheService,
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext)
    : IQueryHandler<GetInvitationLinkQuery, GenericResult<InvitationLinkResponse>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<InvitationLinkResponse>> Handle(GetInvitationLinkQuery request, CancellationToken cancellationToken)
    {
        if (!await _communityRepository.IsOwner(_authenticationContext.UserId, request.CommunityId, cancellationToken))
        {
            return GenericResult<InvitationLinkResponse>.Failure("You are not the community owner");
        }

        string? link = await _cacheService.GetInvitationLink(request.CommunityId, cancellationToken);

        if (link is null)
        {
            return GenericResult<InvitationLinkResponse>.Failure("Invitation link not found");
        }

        return GenericResult<InvitationLinkResponse>.Success(new InvitationLinkResponse(link));
    }
}
