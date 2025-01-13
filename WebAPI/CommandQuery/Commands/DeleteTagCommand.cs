using WebAPI.CQRS;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record DeleteTagCommand(Guid Id) : ICommand<OperationResult<DeleteTagResponse>>;
