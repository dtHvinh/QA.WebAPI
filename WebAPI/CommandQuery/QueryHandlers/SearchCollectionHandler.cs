using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class SearchCollectionHandler(ICollectionRepository collectionRepository)
    : IQueryHandler<SearchCollectionQuery, GenericResult<PagedResponse<GetCollectionResponse>>>
{
    private readonly ICollectionRepository _collectionRepository = collectionRepository;

    public async Task<GenericResult<PagedResponse<GetCollectionResponse>>> Handle(SearchCollectionQuery request, CancellationToken cancellationToken)
    {
        var collections = await _collectionRepository.SearchCollections(
            request.SearchTerm,
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageIndex + 1,
            cancellationToken);

        var hasNext = collections.Count == request.PageArgs.PageSize + 1;
        return GenericResult<PagedResponse<GetCollectionResponse>>.Success(
            new PagedResponse<GetCollectionResponse>(
                collections.Take(request.PageArgs.PageSize).Select(e => e.ToGetCollectionResponse()).ToList(),
                hasNext,
                request.PageArgs.PageIndex,
                request.PageArgs.PageSize));
    }
}
