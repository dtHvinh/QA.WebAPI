using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record UpdateAnswerCommand(UpdateAnswerDto Answer, int AnswerId)
    : ICommand<GenericResult<AnswerResponse>>;
