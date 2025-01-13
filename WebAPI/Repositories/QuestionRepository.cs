using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IQuestionRepository))]
public class QuestionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Question>(dbContext), IQuestionRepository
{
    public async Task<OperationResult<Question>> AddQuestionAsync(Question question,
                                                                  CancellationToken cancellationToken)
    {
        return await AddAsync(question, cancellationToken);
    }
}
