using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record DeleteAnswerCommand(Guid Id) : ICommand<GenericResult<GenericResponse>>;
