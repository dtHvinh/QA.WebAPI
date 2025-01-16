using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record SearchQuestionQuery(string Keyword, Guid TagId, PageArgs Args)
    : IQuery<OperationResult<PagedResponse<GetQuestionResponse>>>;
