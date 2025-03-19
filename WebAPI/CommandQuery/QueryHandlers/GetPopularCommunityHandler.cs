using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetPopularCommunityHandler(ICommunityRepository communityRepository, AuthenticationContext authenticationContext)
    : IQueryHandler<GetPopularCommunityQuery, GenericResult<List<GetCommunityResponse>>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<List<GetCommunityResponse>>> Handle(GetPopularCommunityQuery request, CancellationToken cancellationToken)
    {
        var communities = await _communityRepository.GetPopularCommunitiesWithJoinStatus(
            _authenticationContext.UserId,
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageSize,
            cancellationToken);

        var res = communities.Select(c => new GetCommunityResponse()
        {
            Description = c.Description,
            IconImage = c.IconImage,
            Id = c.Id,
            IsPrivate = c.IsPrivate,
            Name = c.Name,
            MemberCount = c.MemberCount,
            IsJoined = c.IsJoined
        }).ToList();

        return GenericResult<List<GetCommunityResponse>>.Success(res);
    }
}
