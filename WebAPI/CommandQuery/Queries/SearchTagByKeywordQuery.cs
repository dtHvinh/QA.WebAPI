using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record SearchTagByKeywordQuery(string Keyword, PageArgs PageArgs)
    : IQuery<GenericResult<PagedResponse<TagResponse>>>;
