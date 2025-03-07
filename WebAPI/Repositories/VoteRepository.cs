using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IVoteRepository))]
public class VoteRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Vote>(dbContext), IVoteRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<bool> DownvoteQuestion(int questionId, int userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(questionId, userId, false, cancellationToken);

    public Task<bool> UpvoteQuestion(int questionId, int userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(questionId, userId, true, cancellationToken);

    private async Task<bool> InternalVoteQuestion(int questionId, int userId, bool isUpvote, CancellationToken cancellationToken)
    {
        var existVote = await _dbContext.Set<QuestionVote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.QuestionId == questionId,
            cancellationToken);

        if (existVote != null)
        {
            if (existVote.IsUpvote != isUpvote)
            {
                existVote.IsUpvote = isUpvote;
                Entities.Update(existVote);

                return true;
            }

            return false;
        }

        var upvote = new QuestionVote
        {
            QuestionId = questionId,
            AuthorId = userId,
            IsUpvote = isUpvote,
        };

        Entities.Add(upvote);

        return true;
    }


    public Task<bool> DownvoteAnswer(int answerId, int userId, CancellationToken cancellationToken)
    => InternalVoteAnswer(answerId, userId, false, cancellationToken);

    public Task<bool> UpvoteAnswer(int answerId, int userId, CancellationToken cancellationToken)
        => InternalVoteAnswer(answerId, userId, true, cancellationToken);

    private async Task<bool> InternalVoteAnswer(int answerId, int userId, bool isUpvote, CancellationToken cancellationToken)
    {
        var existVote = await _dbContext.Set<AnswerVote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.AnswerId == answerId,
            cancellationToken);

        if (existVote != null)
        {
            if (existVote.IsUpvote != isUpvote)
            {
                existVote.IsUpvote = isUpvote;
                Entities.Update(existVote);

                return true;
            }

            return false;
        }

        var upvote = new AnswerVote
        {
            AnswerId = answerId,
            AuthorId = userId,
            IsUpvote = isUpvote,
        };

        Entities.Add(upvote);

        return true;
    }
}
