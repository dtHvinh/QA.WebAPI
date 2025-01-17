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
}
