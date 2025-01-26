using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IAnswerRepository : IRepository<Answer>
{
    void AddAnswer(Answer answer);
    int CountQuestionAnswer(Guid questionId);
    Task<Answer?> FindAnswerById(Guid id, CancellationToken cancellationToken = default);
    void UpdateAnswer(Answer answer);
}
