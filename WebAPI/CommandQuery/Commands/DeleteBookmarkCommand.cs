using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record DeleteBookmarkCommand(Guid BookmarkId) : ICommand<GenericResult<GenericResponse>>;
