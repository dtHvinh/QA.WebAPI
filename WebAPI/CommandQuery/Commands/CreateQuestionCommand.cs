using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateQuestionCommand(CreateQuestionDto dto) : ICommand<OperationResult<CreateQuestionResponse>>
{
    public CreateQuestionDto Question { get; } = dto;
}
