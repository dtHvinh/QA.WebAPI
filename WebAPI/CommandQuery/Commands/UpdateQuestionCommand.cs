using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record UpdateQuestionCommand(UpdateQuestionDto Question)
    : ICommand<OperationResult<UpdateQuestionResponse>>;
