using MediatR;
using WebAPI.CommandQuery.Queries;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;
public class SearchTagHandler(ITagRepository tagRepository)
    : IRequestHandler<SearchTagByKeywordQuery, GenericResult<PagedResponse<TagResponse>>>

{
    private readonly ITagRepository tagRepository = tagRepository;

    public async Task<GenericResult<PagedResponse<TagResponse>>> Handle(
        SearchTagByKeywordQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Keyword))
        {
            return GenericResult<PagedResponse<TagResponse>>.Success(new([], false, 0, 8));
        }

        var tags = await tagRepository.FindTagsByKeyword(
            request.Keyword,
            (request.PageArgs.PageIndex - 1) * request.PageArgs.PageSize,
            request.PageArgs.PageSize,
            cancellationToken);

        return GenericResult<PagedResponse<TagResponse>>.Success(
            new(tags.Select(e => e.ToTagResonse()), false, 0, 0)
        );
    }
}
