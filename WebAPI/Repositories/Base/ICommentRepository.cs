using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ICommentRepository : IRepository<Comment>
{
    int CountQuestionComment(Guid questionId);
    Task<Comment?> GetCommentByIdAsync(Guid commentId);
    void UpdateComment(Comment comment);
}
