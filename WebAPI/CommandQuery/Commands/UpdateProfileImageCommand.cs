using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record UpdateProfileImageCommand(IFormFile File) : ICommand<GenericResult<TextResponse>>;
