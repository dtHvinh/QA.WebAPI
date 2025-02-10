using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Enums;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IAnswerRepository))]
public class AnswerRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Answer>(dbContext), IAnswerRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void AddAnswer(Answer answer)
    {
        Entities.Add(answer);
        _dbContext.Entry(answer).Reference(e => e.Author).Load();
    }

    public void TryEditAnswer(Answer answer, out string? errMsg)
    {
        if (answer.IsAccepted)
        {
            errMsg = "Can not edit accepted answer";
            return;
        }

        errMsg = null;
        answer.UpdatedAt = DateTime.UtcNow;
        Entities.Update(answer);
    }

    public int CountQuestionAnswer(int questionId)
    {
        return Entities.Where(e => e.QuestionId.Equals(questionId)).Count();
    }

    public async Task<Answer?> FindAnswerById(int id, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Answer?> FindAnswerWithAuthorById(int id, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(e => e.Id.Equals(id)).Include(e => e.Author)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Answer>> GetAnswersAsync(int questionId, CancellationToken cancellation = default)
    {
        return await Entities.Where(e => e.QuestionId.Equals(questionId)).ToListAsync(cancellation);
    }

    public async Task<int> CountUserAnswer(int userId, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(e => e.AuthorId.Equals(userId)).CountAsync(cancellationToken);
    }

    public void TrySoftDeleteAnswer(Answer answer, out string? errMsg)
    {
        if (answer.IsAccepted)
        {
            errMsg = "Can not delete accepted answer";
            return;
        }

        if (answer.Upvote - answer.Downvote > 0)
        {
            errMsg = "Can not delete answer people may find it valuable";
            return;
        }

        errMsg = null;
        answer.SolftDelete();
        Entities.Update(answer);
    }

    public void VoteChange(Answer answer, VoteUpdateTypes updateType, int value)
    {
        switch (updateType)
        {
            case VoteUpdateTypes.CreateNew:
                if (value == 1)
                    answer.Upvote += value;
                else if (value == -1)
                    answer.Downvote -= value; // - plus - eq +

                Entities.Update(answer);
                break;

            case VoteUpdateTypes.ChangeVote:
                answer.Upvote += value;
                answer.Downvote -= value;
                Entities.Update(answer);
                break;

            case VoteUpdateTypes.NoChange:
                break;

            default:
                throw new InvalidOperationException();
        }

        Entities.Update(answer);
    }
}
