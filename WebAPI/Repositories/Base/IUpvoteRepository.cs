
namespace WebAPI.Repositories.Base;

public interface IUpvoteRepository
{
    Task<bool> AddQuestionUpvote(Guid questionId, Guid userId, CancellationToken cancellationToken);
}
