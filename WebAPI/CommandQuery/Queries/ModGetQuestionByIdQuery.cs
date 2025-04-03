using WebAPI.CQRS;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record ModGetQuestionByIdQuery(int QuestionId)
    : IQuery<GenericResult<GetQuestionResponse>>;
