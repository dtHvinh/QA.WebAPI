using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using static WebAPI.Utilities.Enums;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IVoteRepository))]
public class VoteRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Vote>(dbContext), IVoteRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<VoteUpdateTypes> DownvoteQuestion(int questionId, int userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(questionId, userId, false, cancellationToken);

    public Task<VoteUpdateTypes> UpvoteQuestion(int questionId, int userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(questionId, userId, true, cancellationToken);

    private async Task<VoteUpdateTypes> InternalVoteQuestion(int questionId, int userId, bool isUpvote, CancellationToken cancellationToken)
    {
        var existVote = await _dbContext.Set<QuestionVote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.QuestionId == questionId,
            cancellationToken);

        if (existVote != null)
        {
            if (existVote.IsUpvote == isUpvote)
                return VoteUpdateTypes.NoChange;

            else
            {
                existVote.IsUpvote = isUpvote;
                Entities.Update(existVote);

                return VoteUpdateTypes.ChangeVote;
            }
        }

        var upvote = new QuestionVote
        {
            QuestionId = questionId,
            AuthorId = userId,
            IsUpvote = isUpvote,
        };

        Entities.Add(upvote);

        return VoteUpdateTypes.CreateNew;
    }


    public Task<VoteUpdateTypes> DownvoteAnswer(int answerId, int userId, CancellationToken cancellationToken)
    => InternalVoteAnswer(answerId, userId, false, cancellationToken);

    public Task<VoteUpdateTypes> UpvoteAnswer(int answerId, int userId, CancellationToken cancellationToken)
        => InternalVoteAnswer(answerId, userId, true, cancellationToken);

    private async Task<VoteUpdateTypes> InternalVoteAnswer(int answerId, int userId, bool isUpvote, CancellationToken cancellationToken)
    {
        var existVote = await _dbContext.Set<AnswerVote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.AnswerId == answerId,
            cancellationToken);

        if (existVote != null)
        {
            if (existVote.IsUpvote == isUpvote)
                return VoteUpdateTypes.NoChange;

            else
            {
                existVote.IsUpvote = isUpvote;
                Entities.Update(existVote);

                return VoteUpdateTypes.ChangeVote;
            }
        }

        var upvote = new AnswerVote
        {
            AnswerId = answerId,
            AuthorId = userId,
            IsUpvote = isUpvote,
        };

        Entities.Add(upvote);

        return VoteUpdateTypes.CreateNew;
    }
}
