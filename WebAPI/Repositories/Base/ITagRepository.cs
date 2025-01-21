using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ITagRepository : IRepository<Tag>
{
    void CreateTag(Tag tag);
    void CreateTags(List<Tag> tags);
    void DeleteTag(Guid id);
    void DeleteTag(Tag tag);
    Task<List<Tag>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<List<Tag>> FindAllTagByIds(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Tag>> FindTagsByNames(List<string> tagNames, CancellationToken cancellationToken = default);
    Task<List<Guid>> FindTagsIdByNames(List<string> tagNames, CancellationToken cancellationToken = default);
    void UpdateTag(Tag tag, CancellationToken cancellation = default);
}
