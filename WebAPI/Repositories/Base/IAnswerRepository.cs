using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IAnswerRepository : IRepository<Answer>
{
    void AddAnswer(Answer answer);
    int CountQuestionAnswer(Guid questionId);
    void UpdateAnswer(Answer answer);
}
