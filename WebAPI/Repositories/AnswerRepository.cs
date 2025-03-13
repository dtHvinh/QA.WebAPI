using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Extensions;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IAnswerRepository))]
public class AnswerRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Answer>(dbContext), IAnswerRepository
{
    public void AddAnswer(Answer answer)
    {
        Entities.Add(answer);
    }

    public Task AddAnswerAndLoadAuthor(Answer answer, CancellationToken cancellationToken = default)
    {
        Entities.Add(answer);
        return dbContext.Entry(answer).Reference(e => e.Author).LoadAsync(cancellationToken);
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

    public async Task<int> CountUserAcceptedAnswer(int userId, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(e => e.AuthorId.Equals(userId) && e.IsAccepted).CountAsync(cancellationToken);
    }

    public void UpdateAnswer(Answer answer)
    {
        Entities.Update(answer);
    }

    public void TrySoftDeleteAnswer(Answer answer, out string? errMsg)
    {
        if (answer.IsAccepted)
        {
            errMsg = "Can not delete accepted answer";
            return;
        }

        if (answer.Score > 0)
        {
            errMsg = "Can not delete answer people may find it valuable";
            return;
        }

        errMsg = null;
        answer.SolftDelete();
        Entities.Update(answer);
    }
}
