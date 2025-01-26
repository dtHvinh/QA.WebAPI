using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IBookmarkRepository))]
public class BookmarkRepository(ApplicationDbContext dbContext)
    : RepositoryBase<BookMark>(dbContext), IBookmarkRepository
{
    public void AddBookmark(BookMark bookMark)
    {
        Entities.Add(bookMark);
    }

    public void DeleteBookmark(BookMark bookMark)
    {
        Entities.Remove(bookMark);
    }

    public async Task<BookMark?> FindBookmark(Guid userId, Guid questionId)
    {
        return await Table.FirstOrDefaultAsync(e => e.AuthorId.Equals(userId) && e.QuestionId.Equals(questionId));
    }
}
