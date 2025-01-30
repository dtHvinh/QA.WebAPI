using WebAPI.CQRS;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetTagDetailQuery(Guid TagId) : IQuery<GenericResult<TagDetailResponse>>;
