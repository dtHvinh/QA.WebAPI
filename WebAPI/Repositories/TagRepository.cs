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
        Entities.Add(tag);
    }

    public async Task<Tag?> FindTagWithBodyById(int id, CancellationToken cancellationToken = default)
    {
        var tag = await Entities.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        if (tag is null) return null;
        await _dbContext.Entry(tag).Reference(e => e.WikiBody).LoadAsync(cancellationToken);
        await _dbContext.Entry(tag).Reference(e => e.Description).LoadAsync(cancellationToken);
        return tag;
    }

    public async Task<TagDescription?> FindTagDescription(int tagId, CancellationToken cancellationToken = default)
    {
        var tag = await Table.FirstOrDefaultAsync(e => e.Id.Equals(tagId), cancellationToken);
        if (tag is null) return null;
        await _dbContext.Entry(tag).Reference(e => e.Description).LoadAsync(cancellationToken);
        return tag.Description;
    }

    public async Task<List<Tag>> GetQuestionTags(Question question, CancellationToken cancellationToken = default)
    {
        await _dbContext.Entry(question).Collection(e => e.Tags).LoadAsync(cancellationToken);
        return [.. question.Tags];
    }

    public Task<Tag?> FindTagWithQuestionById(int tagId, QuestionSortOrder orderBy, int questionSkip, int questionTake, CancellationToken cancellationToken = default)
    {
        var q = Entities.Where(e => e.Id.Equals(tagId))
                    .Include(e => e.WikiBody)
                    .Include(e => e.Description);

        return orderBy switch
        {
            QuestionSortOrder.Newest => q
                       .Include(e => e.Questions.OrderByDescending(e => e.CreatedAt)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .AsSplitQuery()
                       .Include(e => e.Questions.OrderByDescending(e => e.CreatedAt)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            QuestionSortOrder.MostVoted => q
                       .Include(e => e.Questions.OrderByDescending(e => e.Score)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .AsSplitQuery()
                       .Include(e => e.Questions.OrderByDescending(e => e.Score)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            QuestionSortOrder.MostViewed => q
                       .Include(e => e.Questions.OrderByDescending(e => e.ViewCount)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .AsSplitQuery()
                       .Include(e => e.Questions.OrderByDescending(e => e.ViewCount)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            QuestionSortOrder.Solved => q
                       .Include(e => e.Questions.OrderByDescending(e => e.IsSolved)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .AsSplitQuery()
                       .Include(e => e.Questions.OrderByDescending(e => e.IsSolved)
                                                .Where(e => !e.IsDeleted)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            _ => q
                       .Include(e => e.Questions.Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .AsSplitQuery()
                       .Include(e => e.Questions.Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),
        };
    }

    /// <summary>
    /// The tags query from this method do not include <see cref="Tag.WikiBody"/> and <see cref="Tag.Questions" />.
    /// </summary>
    public Task<List<Tag>> FindTagsAsync(string orderBy, int skip, int take, CancellationToken cancellationToken = default)
    {
        var normalizedOrder = orderBy?.ToLowerInvariant() ?? "";

        var q = normalizedOrder switch
        {
            "popular" => Table.OrderByDescending(e => e.QuestionCount),
            "name" => Table.OrderBy(e => e.Name),
            "newest" => Table.OrderByDescending(e => e.CreatedAt),
            _ => Table.AsNoTracking()
        };

        return q.Skip(skip)
                .Take(take)
                .Select(e => new Tag
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    QuestionCount = e.QuestionCount
                })
                .ToListAsync(cancellationToken);
    }

    public async Task<List<Tag>> FindAllTagByIds(List<int> ids, CancellationToken cancellationToken = default)
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

    public async Task<List<int>> FindTagsIdByNames(
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
            .Include(e => e.Description)
            .ToListAsync(cancellationToken);

        return tagsToFind;
    }

    public void UpdateTag(Tag tag, CancellationToken cancellation = default)
    {
        SetNormalizedTagName(tag);
        Entities.Update(tag);
    }

    public void DeleteTag(int id)
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
