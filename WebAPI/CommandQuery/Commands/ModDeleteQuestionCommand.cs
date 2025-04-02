using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record ModDeleteQuestionCommand(int QuestionId)
    : ICommand<GenericResult<TextResponse>>;
