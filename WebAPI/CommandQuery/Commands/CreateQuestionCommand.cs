using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateQuestionCommand(CreateQuestionDto dto, bool draftMode = false)
    : ICommand<OperationResult<CreateQuestionResponse>>
{
    public CreateQuestionDto Question { get; } = dto;
    public bool DraftMode { get; } = draftMode;
}
