using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICommentRepository))]
public class CommentRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Comment>(dbContext), ICommentRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void AddComment(Comment comment)
    {
        Entities.Add(comment);
    }

    public async Task AddAndLoadAuthor(Comment comment)
    {
        Entities.Add(comment);
        await _dbContext.Entry(comment).Reference(e => e.Author).LoadAsync();
    }

    public int CountQuestionComment(int questionId)
    {
        return _dbContext.Set<QuestionComment>().Where(e => e.QuestionId.Equals(questionId))
             .Count();
    }

    public void UpdateComment(Comment comment)
    {
        comment.ModificationDate = DateTime.UtcNow;
        Entities.Update(comment);
    }
    public Task<int> CountUserComment(int userId, CancellationToken cancellationToken)
    {
        return Table.CountAsync(e => e.AuthorId.Equals(userId), cancellationToken);
    }

    public async Task<Comment?> GetCommentByIdAsync(int commentId)
    {
        return await Entities.FindAsync(commentId);
    }

    public async Task<List<QuestionComment>> GetQuestionCommentWithAuthor(int questionId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuestionComment>().Where(e => e.QuestionId.Equals(questionId))
            .OrderByDescending(e => e.CreationDate)
            .Skip(skip)
            .Take(take)
            .Include(e => e.Author)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
