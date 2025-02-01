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

    public Task<Tag?> FindTagDetailById(Guid tagId, CancellationToken cancellationToken = default)
    {
        return Entities.Where(e => e.Id.Equals(tagId))
                       .Include(e => e.Questions.OrderByDescending(e => e.Upvote)
                                                .ThenByDescending(e => e.ViewCount)
                                                .ThenByDescending(e => e.CreatedAt)
                                                .Take(15))
                       .ThenInclude(e => e.Author)
                       .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// The tags query from this method do not include <see cref="Tag.WikiBody"/> and <see cref="Tag.Questions" />.
    /// </summary>
    public Task<List<Tag>> FindTagsAsync(string orderBy, int skip, int take, CancellationToken cancellationToken = default)
    {
        return orderBy.ToLowerInvariant() switch
        {
            "popular" => Entities.OrderByDescending(e => e.QuestionCount)
                                .Skip(skip)
                                .Take(take)
                                .Select(e => new Tag()
                                {
                                    Id = e.Id,
                                    Name = e.Name,
                                    Description = e.Description,
                                    QuestionCount = e.QuestionCount
                                })
                                .ToListAsync(cancellationToken),

            "name" => Entities.OrderBy(e => e.Name)
                            .Skip(skip)
                            .Take(take)
                            .Select(e => new Tag()
                            {
                                Id = e.Id,
                                Name = e.Name,
                                Description = e.Description,
                                QuestionCount = e.QuestionCount
                            })
                            .ToListAsync(cancellationToken),

            _ => Entities.Skip(skip)
                        .Take(take)
                        .Select(e => new Tag()
                        {
                            Id = e.Id,
                            Name = e.Name,
                            Description = e.Description,
                            QuestionCount = e.QuestionCount
                        })
                        .ToListAsync(cancellationToken),
        };
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

    public async Task<List<Tag>> FindTagsByKeyword(
        string keyword, int skip, int take, CancellationToken cancellationToken = default)
    {
        var findKeyword = keyword.ToUpperInvariant();

        var tagsToFind = await Table
            .Where(e => e.NormalizedName.Contains(findKeyword))
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return tagsToFind;
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
