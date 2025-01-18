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

    public Task<VoteUpdateTypes> DownvoteQuestion(Guid questionId, Guid userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(questionId, userId, false, cancellationToken);

    public Task<VoteUpdateTypes> UpvoteQuestion(Guid questionId, Guid userId, CancellationToken cancellationToken)
        => InternalVoteQuestion(questionId, userId, true, cancellationToken);

    private async Task<VoteUpdateTypes> InternalVoteQuestion(Guid questionId, Guid userId, bool isUpvote, CancellationToken cancellationToken)
    {
        var existVote = await _dbContext.Set<QuestionVote>()
            .FirstOrDefaultAsync(u => u.AuthorId == userId && u.VoteType == VoteTypes.Question.ToString(),
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
}
