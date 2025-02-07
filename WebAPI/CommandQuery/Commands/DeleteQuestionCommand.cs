using WebAPI.CQRS;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record struct DeleteQuestionCommand(int Id) : ICommand<GenericResult<DeleteQuestionResponse>>;
