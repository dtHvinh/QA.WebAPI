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

    public async Task<List<BookMark>> FindUserBookmark(int userId, QuestionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await Table
            .AsNoTracking()
            .Where(e => e.AuthorId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Include(e => e.Question)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountUserBookmark(int userId, CancellationToken cancellationToken = default)
    {
        return await Entities.CountAsync(e => e.AuthorId == userId, cancellationToken);
    }

    public async Task<BookMark?> FindBookmark(int userId, int questionId)
    {
        return await Table.FirstOrDefaultAsync(e => e.AuthorId.Equals(userId) && e.QuestionId.Equals(questionId));
    }

    public async Task<bool> IsBookmarked(int userId, int questionId)
    {
        return await Table.AnyAsync(e => e.AuthorId == userId && e.QuestionId == questionId);
    }
}
