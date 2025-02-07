using WebAPI.CQRS;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetTagDetailQuery(int Id) : IQuery<GenericResult<TagWithWikiBodyResponse>>;
