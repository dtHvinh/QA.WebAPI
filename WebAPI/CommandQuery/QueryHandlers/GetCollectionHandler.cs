using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCollectionHandler(
    ICollectionRepository collectionRepository,
    AuthenticationContext authenticationContext)
    : IQueryHandler<GetCollectionsQuery, GenericResult<PagedResponse<GetCollectionResponse>>>
{
    private readonly ICollectionRepository _collectionRepository = collectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<PagedResponse<GetCollectionResponse>>> Handle(GetCollectionsQuery request,
        CancellationToken cancellationToken)
    {
        var collections = await _collectionRepository.FindCollections(
            request.SortOrder,
            request.Args.CalculateSkip(),
            request.Args.PageSize + 1, cancellationToken);

        var hasNext = collections.Count == request.Args.PageSize + 1;

        var totalCount = await _collectionRepository.PublicCollectionCount(cancellationToken);

        return GenericResult<PagedResponse<GetCollectionResponse>>.Success(
            new PagedResponse<GetCollectionResponse>(
                collections.Take(request.Args.PageSize).Select(e =>
                    e.ToGetCollectionResponse().SetResourceRight(_authenticationContext.UserId)).ToList(),
                hasNext,
                request.Args.PageIndex,
                request.Args.PageSize)
            {
                TotalCount = totalCount,
                TotalPage = MathHelper.GetTotalPage(totalCount, request.Args.PageSize)
            });
    }
}