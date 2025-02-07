using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record SearchQuestionQuery(string Keyword, int TagId, PageArgs Args)
    : IQuery<GenericResult<PagedResponse<GetQuestionResponse>>>;
