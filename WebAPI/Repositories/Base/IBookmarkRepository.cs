using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IBookmarkRepository : IRepository<BookMark>
{
    Task<BookMark?> FindBookmark(int userId, int questionId);
    void AddBookmark(BookMark bookMark);
    void DeleteBookmark(BookMark bookMark);
    Task<bool> IsBookmarked(int userId, int questionId);
    Task<List<BookMark>> FindUserBookmark(int userId, QuestionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken = default);
    Task<int> CountUserBookmark(int userId, CancellationToken cancellationToken = default);
}
