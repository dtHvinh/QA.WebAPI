using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCommunityDetailHandler(ICommunityRepository communityRepository, AuthenticationContext authenticationContext)
    : IQueryHandler<GetCommunityDetailQuery, GenericResult<GetCommunityDetailResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GetCommunityDetailResponse>> Handle(GetCommunityDetailQuery request, CancellationToken cancellationToken)
    {
        var community = await _communityRepository.GetCommunityDetailByNameAsync(request.CommunityName, cancellationToken);

        if (community is null)
            return GenericResult<GetCommunityDetailResponse>.Failure("Community not found.");

        var isMember = await _communityRepository.IsMember(_authenticationContext.UserId, community.Id, cancellationToken);

        if (!isMember)
            return GenericResult<GetCommunityDetailResponse>.Failure("You are not community member.");

        var memberCount = await _communityRepository.GetMemberCount(community.Id, cancellationToken);

        var isJoined = await _communityRepository.IsJoined(_authenticationContext.UserId, community.Id, cancellationToken);

        var isOwner = await _communityRepository.IsOwner(_authenticationContext.UserId, community.Id, cancellationToken);

        var isModerator = await _communityRepository.IsModerator(_authenticationContext.UserId, community.Id, cancellationToken);

        var res = community.ToDetailResponse()
            .WithIsJoined(isJoined)
            .WithRoles(isOwner, isModerator)
            .WithMemberCount(memberCount);

        return GenericResult<GetCommunityDetailResponse>.Success(res);
    }
}
