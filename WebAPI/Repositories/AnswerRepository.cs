using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IAnswerRepository))]
public class AnswerRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Answer>(dbContext), IAnswerRepository
{
    public void AddAnswer(Answer answer)
    {
        Entities.Add(answer);
    }

    public void UpdateAnswer(Answer answer)
    {
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
}
