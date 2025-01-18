using WebAPI.CQRS;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateQuestionUpvoteCommand(Guid QuestionId) : ICommand<GenericResult<GenericResponse>>;
