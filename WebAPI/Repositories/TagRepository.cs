using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ITagRepository))]
public class TagRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Tag>(dbContext), ITagRepository
{
    public void CreateTags(List<Tag> tags)
    {
        tags.ForEach(SetNormalizedTagName);
        AddRange(tags);
    }

    public void CreateTag(Tag tag)
    {
        SetNormalizedTagName(tag);
        Entities.Add(tag);
    }

    public async Task<List<Tag>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        return await Entities.ToListAsync(cancellationToken);
    }

    public async Task<List<Tag>> FindAllTagByIds(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<List<Tag>> FindTagsByNames(
        List<string> tagNames, CancellationToken cancellationToken = default)
    {
        var tagsToFind = tagNames.Select(e => e.ToUpperInvariant());

        return await Entities.Where(
             x => tagsToFind.Contains(x.NormalizedName)).ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> FindTagsIdByNames(
        List<string> tagNames, CancellationToken cancellationToken = default)
    {
        var tagsToFind = tagNames.Select(e => e.ToUpperInvariant());

        return await Entities.Where(
            x => tagsToFind.Contains(x.NormalizedName)).Select(e => e.Id).ToListAsync(cancellationToken);
    }

    public void UpdateTag(Tag tag, CancellationToken cancellation = default)
    {
        SetNormalizedTagName(tag);
        Entities.Update(tag);
    }

    public void DeleteTag(Guid id)
    {
        var tag = Table.FirstOrDefault(x => x.Id == id);
        if (tag != null)
            Entities.Remove(tag);
    }

    public void DeleteTag(Tag tag)
    {
        Entities.Remove(tag);
    }

    private static void SetNormalizedTagName(Tag tag) => tag.NormalizedName = tag.Name.ToUpperInvariant();
}
