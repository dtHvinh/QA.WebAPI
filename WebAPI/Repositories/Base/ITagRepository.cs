using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ITagRepository : IRepository<Tag>
{
    void CreateTag(Tag tag);
    void CreateTags(List<Tag> tags);
    void DeleteTag(Guid id);
    void DeleteTag(Tag tag);
    /// <summary>
    /// The tags query from this method do not include <see cref="Tag.WikiBody"/>.
    /// </summary>
    Task<List<Tag>> FindTagsAsync(string orderBy, int skip, int take, CancellationToken cancellationToken = default);
    Task<List<Tag>> FindAllTagByIds(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<Tag?> FindTagDetailById(Guid tagId, CancellationToken cancellationToken = default);
    Task<List<Tag>> FindTagsByKeyword(string keyword, int skip, int take, CancellationToken cancellationToken = default);
    Task<List<Tag>> FindTagsByNames(List<string> tagNames, CancellationToken cancellationToken = default);
    Task<List<Guid>> FindTagsIdByNames(List<string> tagNames, CancellationToken cancellationToken = default);
    void UpdateTag(Tag tag, CancellationToken cancellation = default);
}
