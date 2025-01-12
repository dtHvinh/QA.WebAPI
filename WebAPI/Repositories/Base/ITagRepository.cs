using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface ITagRepository
{
    Task<OperationResult<Tag>> AddTagAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<OperationResult> AddTagsAsync(List<Tag> tags, CancellationToken cancellationToken = default);
    Task<OperationResult> AddTagsToQuestionAsync(Question question, List<Tag> tags, CancellationToken cancellationToken = default);
    Task<OperationResult<List<Tag>>> FindTagsByNames(List<string> tagNames, CancellationToken cancellationToken = default);
}
