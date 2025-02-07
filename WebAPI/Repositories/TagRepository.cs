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


    public async Task<List<Tag>> GetQuestionTags(Question question, CancellationToken cancellationToken = default)
    {
        await _dbContext.Entry(question).Collection(e => e.Tags).LoadAsync(cancellationToken);
        return [.. question.Tags];
    }

    public Task<Tag?> FindTagWithQuestionById(int tagId, QuestionSortOrder orderBy, int questionSkip, int questionTake, CancellationToken cancellationToken = default)
    {
        return orderBy switch
        {
            QuestionSortOrder.Newest => Entities.Where(e => e.Id.Equals(tagId))
                       .Include(e => e.Questions.OrderByDescending(e => e.CreatedAt)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .Include(e => e.Questions.OrderByDescending(e => e.CreatedAt)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            QuestionSortOrder.MostVoted => Entities.Where(e => e.Id.Equals(tagId))
                       .Include(e => e.Questions.OrderByDescending(e => e.Upvote - e.Downvote)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .Include(e => e.Questions.OrderByDescending(e => e.Upvote - e.Downvote)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            QuestionSortOrder.MostViewed => Entities.Where(e => e.Id.Equals(tagId))
                       .Include(e => e.Questions.OrderByDescending(e => e.ViewCount)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .Include(e => e.Questions.OrderByDescending(e => e.ViewCount)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            QuestionSortOrder.Solved => Entities.Where(e => e.Id.Equals(tagId))
                       .Include(e => e.Questions.OrderByDescending(e => e.IsSolved)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
                       .Include(e => e.Questions.OrderByDescending(e => e.IsSolved)
                                                .Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Tags)
                       .FirstOrDefaultAsync(cancellationToken),

            _ => Entities.Where(e => e.Id.Equals(tagId))
                       .Include(e => e.Questions.Skip(questionSkip)
                                                .Take(questionTake))
                       .ThenInclude(e => e.Author)
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
