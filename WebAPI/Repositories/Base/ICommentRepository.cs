using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ICommentRepository : IRepository<Comment>
{
    int CountQuestionComment(Guid questionId);
}
