using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteBookmarkHandler(IBookmarkRepository bookmarkRepository, AuthenticationContext authenticationContext) : ICommandHandler<DeleteBookmarkCommand, GenericResult<GenericResponse>>
{
    private readonly IBookmarkRepository _bookmarkRepository = bookmarkRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(DeleteBookmarkCommand request, CancellationToken cancellationToken)
    {
        var bookmark = await _bookmarkRepository.FindFirstAsync(e => e.Id == request.BookmarkId, cancellationToken);

        if (bookmark is null)
        {
            return GenericResult<GenericResponse>.Failure(EM.BOOKMARK_NOT_FOUND);
        }

        if (!_authenticationContext.IsResourceOwnedByUser(bookmark))
        {
            return GenericResult<GenericResponse>.Failure(EM.ACTION_REQUIRE_RESOURCE_OWNER);
        }

        _bookmarkRepository.Remove(bookmark);

        var result = await _bookmarkRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success("Delete successfully")
            : GenericResult<GenericResponse>.Failure(result.Message);

    }
}
