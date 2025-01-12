using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Attributes;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ITagRepository))]
public class TagRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Tag>(dbContext), ITagRepository
{
    public async Task<OperationResult> AddTagsAsync(
        List<Tag> tags, CancellationToken cancellationToken = default)
    {
        var result = await AddRangeAsync(tags, cancellationToken);
        if (!result.IsSuccess)
        {
            return OperationResult.Failure(result.Message);
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult<Tag>> AddTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        var result = await AddAsync(tag, cancellationToken);
        if (!result.IsSuccess)
        {
            return OperationResult<Tag>.Failure(result.Message);
        }

        return OperationResult<Tag>.Success(result.Value!);
    }

    public async Task<OperationResult> AddTagsToQuestionAsync(
        Question question, List<Tag> tags, CancellationToken cancellationToken = default)
    {
        var result = await AddRangeAsync(tags, cancellationToken);
        if (!result.IsSuccess)
        {
            return OperationResult.Failure(result.Message);
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult<List<Tag>>> FindTagsByNames(List<string> tagNames, CancellationToken cancellationToken = default)
    {
        var result = await Entities.Where(x => tagNames.Contains(x.Name)).ToListAsync(cancellationToken);

        return OperationResult<List<Tag>>.Success(result);
    }
}
