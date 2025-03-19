using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetJoinedCommunitiesHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext)
    : IQueryHandler<GetJoinedCommunitiesQuery, GenericResult<List<GetCommunityResponse>>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<List<GetCommunityResponse>>> Handle(GetJoinedCommunitiesQuery request, CancellationToken cancellationToken)
    {
        var communities = await _communityRepository.GetCommunityUserJoined(_authenticationContext.UserId, request.PageArgs.CalculateSkip(), request.PageArgs.PageSize, cancellationToken);

        return GenericResult<List<GetCommunityResponse>>.Success(communities.Select(c => c.ToResponse().WithIsJoined(true)).ToList());
    }
}
