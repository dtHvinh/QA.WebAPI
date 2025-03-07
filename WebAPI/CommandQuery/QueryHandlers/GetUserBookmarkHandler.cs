using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.BookmarkResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetUserBookmarkHandler(
    AuthenticationContext authenticationContext,
    IBookmarkRepository bookmarkRepository) : IQueryHandler<GetUserBookmarkQuery, GenericResult<PagedResponse<BookmarkResponse>>>
{
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly IBookmarkRepository _bookmarkRepository = bookmarkRepository;

    public async Task<GenericResult<PagedResponse<BookmarkResponse>>> Handle(GetUserBookmarkQuery request, CancellationToken cancellationToken)
    {
        var bookmarks = await _bookmarkRepository.FindUserBookmark(
            _authenticationContext.UserId,
            Model.QuestionSortOrder.Newest,
            (request.PageArgs.Page - 1) * request.PageArgs.Page,
            request.PageArgs.PageSize + 1,
            cancellationToken);


        var hasNext = bookmarks.Count == request.PageArgs.PageSize + 1;

        var userBookmarkCount = await _bookmarkRepository.CountUserBookmark(_authenticationContext.UserId, cancellationToken);

        return GenericResult<PagedResponse<BookmarkResponse>>.Success(
            new PagedResponse<BookmarkResponse>(
                bookmarks.Take(request.PageArgs.PageSize).Select(e => e.ToBookmarkResponse()).ToList(),
                hasNext,
                request.PageArgs.Page,
                request.PageArgs.PageSize)
            {
                TotalCount = userBookmarkCount,
                TotalPage = MathHelper.GetTotalPage(userBookmarkCount, request.PageArgs.PageSize)
            });
    }
}
