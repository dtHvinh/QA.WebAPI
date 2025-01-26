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

    public int CountQuestionAnswer(Guid questionId)
    {
        return Entities.Where(e => e.QuestionId.Equals(questionId)).Count();
    }

    public async Task<Answer?> FindAnswerById(Guid id, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
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
}
