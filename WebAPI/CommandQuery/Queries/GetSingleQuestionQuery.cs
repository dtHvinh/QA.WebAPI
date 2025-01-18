using WebAPI.CQRS;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public class GetSingleQuestionQuery(Guid id) : IQuery<GenericResult<GetQuestionResponse>>
{
    public Guid Id { get; } = id;
}
