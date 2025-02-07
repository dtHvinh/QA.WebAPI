using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetTagWithQuestionQuery(int TagId, string OrderBy, PageArgs PageArgs) : IQuery<GenericResult<TagWithQuestionResponse>>;
