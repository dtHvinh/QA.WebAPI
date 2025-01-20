using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateQuestionCommand(CreateQuestionDto dto, bool draftMode = false)
    : ICommand<GenericResult<CreateQuestionResponse>>
{
    public CreateQuestionDto Question { get; } = dto;
    public bool DraftMode { get; } = draftMode;
}
