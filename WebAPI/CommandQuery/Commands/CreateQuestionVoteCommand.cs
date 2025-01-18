using WebAPI.CQRS;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateQuestionVoteCommand(Guid QuestionId, bool IsUpvote) : ICommand<GenericResult<GenericResponse>>;
