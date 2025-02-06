using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IBookmarkRepository : IRepository<BookMark>
{
    Task<BookMark?> FindBookmark(Guid userId, Guid questionId);
    void AddBookmark(BookMark bookMark);
    void DeleteBookmark(BookMark bookMark);
    Task<bool> IsBookmarked(Guid userId, Guid questionId);
    Task<List<BookMark>> FindUserBookmark(Guid userId, QuestionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken = default);
    Task<int> CountUserBookmark(Guid userId, CancellationToken cancellationToken = default);
}
