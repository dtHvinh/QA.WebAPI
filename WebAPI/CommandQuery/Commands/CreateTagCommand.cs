using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateTagCommand(CreateTagDto Tag) : ICommand<OperationResult<CreateTagResponse>>;
