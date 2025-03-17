using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetJoinedCommunitiesQuery(PageArgs PageArgs) : IQuery<GenericResult<List<GetCommunityResponse>>>;
