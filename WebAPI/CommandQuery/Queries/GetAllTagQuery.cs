using WebAPI.CQRS;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public class GetAllTagQuery : IQuery<GenericResult<List<TagResponse>>>
{
}
