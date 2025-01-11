using CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateQuestionCommand(CreateQuestionDto dto) : ICommand<ResultBase<CreateQuestionDto>>
{
    public CreateQuestionDto Question { get; } = dto;
}
