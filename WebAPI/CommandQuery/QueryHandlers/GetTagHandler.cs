using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagHandler(ITagRepository tagRepository) : IQueryHandler<GetTagQuery, GenericResult<PagedResponse<TagResponse>>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<PagedResponse<TagResponse>>> Handle(GetTagQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.FindTagsAsync(
                request.OrderBy,
                request.Skip,
                request.Take + 1,
                cancellationToken);

        var tagResponses = tags.Select(tag => tag.ToTagResonse()).ToList();

        var hasNext = tags.Count == request.Take + 1;

        if (hasNext)
            tagResponses.RemoveAt(tagResponses.Count - 1);

        var totalCount = await _tagRepository.CountAsync();

        return GenericResult<PagedResponse<TagResponse>>.Success(
            new(tagResponses, hasNext, request.Skip / request.Take + 1, request.Take)
            {
                TotalCount = totalCount,
                TotalPage = MathHelper.GetTotalPage(totalCount, request.Take)
            });
    }
}
