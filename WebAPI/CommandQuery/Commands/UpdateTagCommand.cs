using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record UpdateTagCommand(UpdateTagDto Tag) : ICommand<OperationResult<UpdateTagResponse>>;
