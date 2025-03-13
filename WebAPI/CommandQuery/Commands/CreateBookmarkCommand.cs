using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateBookmarkCommand(int QuestionId) : ICommand<GenericResult<TextResponse>>;
