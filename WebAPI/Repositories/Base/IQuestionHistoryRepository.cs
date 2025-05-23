using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IQuestionHistoryRepository : IRepository<QuestionHistory>
{
    void AddHistory(QuestionHistory history);
    Task AddHistory(int questionId, int authorId, string questionHistoryType, string comment, CancellationToken cancellationToken = default);
    Task<List<QuestionHistory>> FindHistoryWithAuthor(int questionId, CancellationToken cancellationToken);
}
