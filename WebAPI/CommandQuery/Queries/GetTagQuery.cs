using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetTagQuery(string OrderBy, int Skip, int Take) : IQuery<GenericResult<PagedResponse<TagResponse>>>;
