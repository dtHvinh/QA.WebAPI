using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IBookmarkRepository : IRepository<BookMark>
{
    Task<BookMark?> FindBookmark(Guid userId, Guid questionId);
    void AddBookmark(BookMark bookMark);
    void DeleteBookmark(BookMark bookMark);
}
