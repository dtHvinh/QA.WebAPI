using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateBookmarkHandler(IBookmarkRepository bookmarkRepository, AuthenticationContext authenticationContext)
    : ICommandHandler<CreateBookmarkCommand, GenericResult<GenericResponse>>
{
    private readonly IBookmarkRepository _bookmarkRepository = bookmarkRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(
        CreateBookmarkCommand request, CancellationToken cancellationToken)
    {
        var existBookmark = await _bookmarkRepository.FindBookmark(_authenticationContext.UserId, request.QuestionId);

        if (existBookmark is not null)
            return GenericResult<GenericResponse>.Failure("You already bookmarked this question");

        var newBookmark = new BookMark()
        {
            AuthorId = _authenticationContext.UserId,
            QuestionId = request.QuestionId
        };

        _bookmarkRepository.AddBookmark(newBookmark);

        var op = await _bookmarkRepository.SaveChangesAsync(cancellationToken);

        return op.IsSuccess
            ? GenericResult<GenericResponse>.Success("Bookmark created")
            : GenericResult<GenericResponse>.Failure(op.Message);
    }
}
