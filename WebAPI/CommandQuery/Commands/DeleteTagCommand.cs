using WebAPI.CQRS;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record DeleteTagCommand(int Id) : ICommand<GenericResult<DeleteTagResponse>>;
