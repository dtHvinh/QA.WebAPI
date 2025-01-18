using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IUpvoteRepository))]
public class UpvoteRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Upvote>(dbContext), IUpvoteRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<bool> AddQuestionUpvote(Guid questionId, Guid userId, CancellationToken cancellationToken)
    {
        var existUpvote = await _dbContext.Set<QuestionUpvote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.UpvoteType == UpvoteTypes.Question.ToString(),
            cancellationToken);

        if (existUpvote != null)
        {
            return false;
        }

        var upvote = new QuestionUpvote
        {
            QuestionId = questionId,
            AuthorId = userId
        };

        Entities.Add(upvote);

        return true;
    }
}
