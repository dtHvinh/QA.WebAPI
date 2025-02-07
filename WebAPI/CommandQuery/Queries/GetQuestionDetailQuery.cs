using WebAPI.CQRS;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetQuestionDetailQuery(int Id) : IQuery<GenericResult<GetQuestionResponse>>;
