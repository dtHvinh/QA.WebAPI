using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record AcceptAnswerCommand(int QuestionId, int AnswerId) : ICommand<GenericResult<GenericResponse>>;
