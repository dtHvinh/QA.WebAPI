using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CloseQuestionCommand(Guid QuestionId) : ICommand<GenericResult<GenericResponse>>;
