using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response.AsnwerResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateAnswerCommand(CreateAnswerDto Answer, Guid QuestionId)
    : ICommand<GenericResult<AnswerResponse>>;
