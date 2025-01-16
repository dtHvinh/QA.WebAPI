using WebAPI.CQRS;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public class GetSingleQuestionQuery(Guid id) : IQuery<OperationResult<GetQuestionResponse>>
{
    public Guid Id { get; } = id;
}
