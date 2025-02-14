using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetSimilarQuestionQuery(int QuestionId, int Skip, int Take) : IQuery<GenericResult<PagedResponse<GetQuestionResponse>>>;
