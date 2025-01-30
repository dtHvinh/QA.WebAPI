using WebAPI.Model;
using WebAPI.Utilities;

namespace WebAPI.Repositories.Base;

public interface IAnswerRepository : IRepository<Answer>
{
    void AddAnswer(Answer answer);
    int CountQuestionAnswer(Guid questionId);
    Task<Answer?> FindAnswerById(Guid id, CancellationToken cancellationToken = default);
    void TrySoftDeleteAnswer(Answer answer, out string? errMsg);
    void TryEditAnswer(Answer answer, out string? errMsg);
    void VoteChange(Answer answer, Enums.VoteUpdateTypes updateType, int value);
}
