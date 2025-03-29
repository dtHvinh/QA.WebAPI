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

    public Task<bool> DownvoteQuestion(Question question, int userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(question, userId, false, cancellationToken);

    public Task<bool> UpvoteQuestion(Question question, int userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(question, userId, true, cancellationToken);

    private async Task<bool> InternalVoteQuestion(Question question, int userId, bool isUpvote, CancellationToken cancellationToken)
    {
        var existVote = await _dbContext.Set<QuestionVote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.QuestionId == question.Id,
            cancellationToken);

        if (existVote != null)
        {
            if (existVote.IsUpvote != isUpvote)
            {
                existVote.IsUpvote = isUpvote;
                Entities.Update(existVote);

                question.Score += isUpvote ? 2 : -2;

                return true;
            }

            return false;
        }

        var upvote = new QuestionVote
        {
            QuestionId = question.Id,
            AuthorId = userId,
            IsUpvote = isUpvote,
        };

        question.Score += isUpvote ? 1 : -1;

        Entities.Add(upvote);

        return true;
    }


    public Task<bool> DownvoteAnswer(Answer answer, int userId, CancellationToken cancellationToken)
    => InternalVoteAnswer(answer, userId, false, cancellationToken);

    public Task<bool> UpvoteAnswer(Answer answer, int userId, CancellationToken cancellationToken)
        => InternalVoteAnswer(answer, userId, true, cancellationToken);

    private async Task<bool> InternalVoteAnswer(Answer answer, int userId, bool isUpvote, CancellationToken cancellationToken)
    {
        var existVote = await _dbContext.Set<AnswerVote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.AnswerId == answer.Id,
            cancellationToken);

        if (existVote != null)
        {
            if (existVote.IsUpvote != isUpvote)
            {
                existVote.IsUpvote = isUpvote;
                Entities.Update(existVote);

                answer.Score += isUpvote ? 2 : -2;

                return true;
            }

            return false;
        }

        var upvote = new AnswerVote
        {
            AnswerId = answer.Id,
            AuthorId = userId,
            IsUpvote = isUpvote,
        };

        answer.Score += isUpvote ? 1 : -1;

        Entities.Add(upvote);

        return true;
    }
}
