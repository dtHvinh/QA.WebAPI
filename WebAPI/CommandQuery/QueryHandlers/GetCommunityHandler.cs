using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCommunityHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext)
    : IQueryHandler<GetCommunityQuery, GenericResult<List<GetCommunityResponse>>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<List<GetCommunityResponse>>> Handle(GetCommunityQuery request, CancellationToken cancellationToken)
    {
        var communities = await _communityRepository.GetCommunitiesWithJoinStatusAsync(
            _authenticationContext.UserId,
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageSize,
            cancellationToken);

        var res = communities.Select(c => c.ToResponse().WithIsJoined(c.IsJoined)).ToList();

        return GenericResult<List<GetCommunityResponse>>.Success(res);
    }
}
