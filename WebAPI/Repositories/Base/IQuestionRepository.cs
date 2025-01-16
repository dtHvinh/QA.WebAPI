using WebAPI.Model;
using WebAPI.Utilities.Params;

namespace WebAPI.Repositories.Base;

public interface IQuestionRepository : IRepositoryBase<Question>
{
    Task<Question?> FindAvailableQuestionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Question>> SearchQuestionAsync(QuestionSearchParams searchParams, CancellationToken cancellationToken);
    void MarkAsView(Guid questionId);
    Task SetQuestionTag(Question question, List<Tag> tags);
}
