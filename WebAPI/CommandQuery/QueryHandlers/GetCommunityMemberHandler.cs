using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCommunityMemberHandler(ICommunityRepository communityRepository)
    : IQueryHandler<GetCommunityMemberQuery, GenericResult<PagedResponse<CommunityMemberResponse>>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;

    public async Task<GenericResult<PagedResponse<CommunityMemberResponse>>> Handle(GetCommunityMemberQuery request, CancellationToken cancellationToken)
    {
        var mem = await _communityRepository.GetMembers(
            request.Communityid,
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageSize + 1, cancellationToken);

        var hasNext = mem.Count == request.PageArgs.PageSize + 1;

        var items = mem.Take(request.PageArgs.PageSize).Select(m => m.ToResponse()).ToList();

        var totalCount = await _communityRepository.GetMemberCount(request.Communityid, cancellationToken);

        var res = new PagedResponse<CommunityMemberResponse>(items, hasNext, request.PageArgs.PageIndex, request.PageArgs.PageSize)
        {
            TotalCount = totalCount,
            TotalPage = MathHelper.GetTotalPage(totalCount, request.PageArgs.PageSize)
        };

        return GenericResult<PagedResponse<CommunityMemberResponse>>.Success(res);
    }
}
