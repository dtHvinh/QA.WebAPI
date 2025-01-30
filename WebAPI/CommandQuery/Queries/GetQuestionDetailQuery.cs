using WebAPI.CQRS;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public class GetQuestionDetailQuery(Guid id) : IQuery<GenericResult<GetQuestionResponse>>
{
    public Guid Id { get; } = id;
}
