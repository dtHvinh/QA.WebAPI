using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ICommentRepository : IRepository<Comment>
{
    Task AddAndLoadAuthor(Comment comment);
    int CountQuestionComment(int questionId);
    Task<int> CountUserComment(int userId, CancellationToken cancellationToken);
    Task<Comment?> GetCommentByIdAsync(int commentId);
    Task<List<QuestionComment>> GetQuestionCommentWithAuthor(int questionId, int skip, int take, CancellationToken cancellationToken = default);
    void UpdateComment(Comment comment);
}
