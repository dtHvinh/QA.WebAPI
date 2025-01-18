using WebAPI.CQRS;
using WebAPI.Utilities.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record DeleteTagCommand(Guid Id) : ICommand<GenericResult<DeleteTagResponse>>;
