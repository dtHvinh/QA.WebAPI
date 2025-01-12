using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IQuestionRepository
{
    Task<OperationResult<Question>> AddQuestionAsync(Question question, CancellationToken cancellationToken);
}
