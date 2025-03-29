using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IVoteRepository
{
    Task<bool> UpvoteQuestion(Question question, int userId, CancellationToken cancellationToken);
    Task<bool> DownvoteQuestion(Question question, int userId, CancellationToken cancellationToken);
    Task<bool> DownvoteAnswer(Answer answer, int userId, CancellationToken cancellationToken);
    Task<bool> UpvoteAnswer(Answer answer, int userId, CancellationToken cancellationToken);
}
