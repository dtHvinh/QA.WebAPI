using WebAPI.CQRS;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetCommunityDetailQuery(string CommunityName)
    : IQuery<GenericResult<GetCommunityDetailResponse>>;
