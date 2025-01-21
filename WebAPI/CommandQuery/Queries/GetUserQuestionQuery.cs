using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetUserQuestionQuery(PageArgs Args, string Order)
    : IQuery<GenericResult<PagedResponse<GetQuestionResponse>>>;
