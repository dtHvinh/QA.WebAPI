using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetQuestionQuery(string Keyword, string Tag, PageArgs Args)
    : IQuery<OperationResult<PagedResponse<GetQuestionResponse>>>;
