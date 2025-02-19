using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetQuestionYouMayLikeQuery(PageArgs PageArgs) : IQuery<GenericResult<PagedResponse<GetQuestionResponse>>>;
