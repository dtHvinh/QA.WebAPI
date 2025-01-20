using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record UpdateQuestionCommand(UpdateQuestionDto Question)
    : ICommand<GenericResult<UpdateQuestionResponse>>;
