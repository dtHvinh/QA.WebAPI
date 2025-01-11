using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Attributes;

namespace WebAPI.Repositories;

[RepositoryImplOf(Type = typeof(IQuestionRepository))]
public class QuestionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Question>(dbContext), IQuestionRepository
{

}
