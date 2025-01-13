using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface ITagRepository
{
    Task<OperationResult<Tag>> CreateTagAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<OperationResult> CreateTagsAsync(List<Tag> tags, CancellationToken cancellationToken = default);
    Task<OperationResult> AddQuestionToTagsAsync(Question question, List<Guid> tagIds, CancellationToken cancellationToken = default);
    Task<OperationResult<List<Tag>>> FindTagsByNames(List<string> names, CancellationToken cancellationToken = default);
    Task<OperationResult<Tag>> UpdateTagAsync(Tag tag, CancellationToken cancellation = default);
    Task<OperationResult<Tag>> DeleteTagAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult<List<Guid>>> FindTagsIdByNames(List<string> names, CancellationToken cancellationToken = default);
}
