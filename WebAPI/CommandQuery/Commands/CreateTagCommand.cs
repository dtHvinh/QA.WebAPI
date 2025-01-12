using CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public class CreateTagCommand(CreateTagDto tag) : ICommand<OperationResult<CreateTagResponse>>
{
    public CreateTagDto Tag { get; } = tag;
}
