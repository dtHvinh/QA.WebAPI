using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateQuestionCommand(CreateQuestionDto dto)
    : ICommand<GenericResult<CreateQuestionResponse>>
{
    public CreateQuestionDto Question { get; } = dto;
}
