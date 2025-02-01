using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagHandler(ITagRepository tagRepository, ICacheService cacheService) : IQueryHandler<GetTagQuery, GenericResult<PagedResponse<TagResponse>>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GenericResult<PagedResponse<TagResponse>>> Handle(GetTagQuery request, CancellationToken cancellationToken)
    {
        List<Tag>? tags;

        tags = await _cacheService.GetTags(request.OrderBy, request.Skip, request.Take + 1);

        if (tags is null)
        {
            tags = await _tagRepository.FindTagsAsync(
                request.OrderBy,
                request.Skip,
                request.Take + 1,
                cancellationToken);

            await _cacheService.SetTags(request.OrderBy, request.Skip, request.Take + 1, tags);
        }

        var tagResponses = tags.Select(tag => tag.ToTagResonse()).ToList();

        var hasNext = tags.Count == request.Take + 1;

        if (hasNext)
            tagResponses.RemoveAt(tagResponses.Count - 1);

        var totalCount = _tagRepository.CountAll();

        return GenericResult<PagedResponse<TagResponse>>.Success(
            new(tagResponses, hasNext, request.Skip / request.Take + 1, request.Take)
            {
                TotalCount = totalCount,
                TotalPage = totalCount / request.Take
            });
    }
}
