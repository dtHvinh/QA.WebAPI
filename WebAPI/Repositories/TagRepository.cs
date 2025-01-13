using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ITagRepository))]
public class TagRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Tag>(dbContext), ITagRepository
{
    public async Task<OperationResult> CreateTagsAsync(
        List<Tag> tags, CancellationToken cancellationToken = default)
    {
        tags.ForEach(SetNormalizedTagName);
        var result = await AddRangeAsync(tags, cancellationToken);
        return result;
    }

    public async Task<OperationResult<Tag>> CreateTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        SetNormalizedTagName(tag);
        var result = await AddAsync(tag, cancellationToken);
        return result;
    }

    public async Task<OperationResult> AddTagsToQuestionAsync(
        Question question, List<Tag> tags, CancellationToken cancellationToken = default)
    {
        var result = await AddRangeAsync(tags, cancellationToken);
        return result;
    }

    public async Task<OperationResult<List<Tag>>> FindTagsByNames(
        List<string> tagNames, CancellationToken cancellationToken = default)
    {
        var tagsToFind = tagNames.Select(e => e.ToUpperInvariant());

        var result = await Entities.Where(
            x => tagsToFind.Contains(x.NormalizedName)).ToListAsync(cancellationToken);

        return OperationResult<List<Tag>>.Success(result);
    }

    public async Task<OperationResult<Tag>> UpdateTagAsync(Tag tag, CancellationToken cancellation = default)
    {
        SetNormalizedTagName(tag);
        var result = await UpdateAsync(tag, cancellation);
        return result;
    }

    public async Task<OperationResult<Tag>> DeleteTagAsync(Guid id, CancellationToken cancellationToken)
    {
        var findResult = await FindFirstAsync(e => e.Id.Equals(id), cancellationToken: cancellationToken);
        if (!findResult.IsSuccess)
        {
            return findResult;
        }

        var tagToDel = findResult.Value!;
        await RemoveAsync(tagToDel, cancellationToken);

        return findResult;
    }

    private static void SetNormalizedTagName(Tag tag) => tag.NormalizedName = tag.Name.ToUpperInvariant();
}
