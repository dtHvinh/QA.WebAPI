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
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void CreateTags(List<Tag> tags)
    {
        tags.ForEach(SetNormalizedTagName);
        AddRange(tags);
    }

    public void CreateTag(Tag tag)
    {
        SetNormalizedTagName(tag);
        Add(tag);
    }

    public void AddQuestionToTags(Question question, List<Guid> tagIds)
    {
        var questionId = question.Id;
        if (tagIds != null)
            _dbContext.Set<QuestionTag>().AddRange(
              tagIds.Select(e => new QuestionTag
              {
                  QuestionId = questionId,
                  TagId = e
              }));
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
        Update(tag);
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
