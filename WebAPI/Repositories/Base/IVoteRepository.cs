namespace WebAPI.Repositories.Base;

public interface IVoteRepository
{
    Task<bool> UpvoteQuestion(int questionId, int userId, CancellationToken cancellationToken);
    Task<bool> DownvoteQuestion(int questionId, int userId, CancellationToken cancellationToken);
    Task<bool> DownvoteAnswer(int answerId, int userId, CancellationToken cancellationToken);
    Task<bool> UpvoteAnswer(int answerId, int userId, CancellationToken cancellationToken);
}
