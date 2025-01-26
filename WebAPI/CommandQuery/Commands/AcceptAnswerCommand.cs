using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record AcceptAnswerCommand(Guid QuestionId, Guid AnswerId) : ICommand<GenericResult<GenericResponse>>;
