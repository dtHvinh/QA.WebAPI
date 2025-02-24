using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetUserCollectionHandler(ICollectionRepository qcr, AuthenticationContext authenticationContext) : IQueryHandler<GetUserCollectionQuery, GenericResult<PagedResponse<GetCollectionResponse>>>
{
    private readonly ICollectionRepository _qcr = qcr;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<PagedResponse<GetCollectionResponse>>> Handle(GetUserCollectionQuery request, CancellationToken cancellationToken)
    {
        var query = await _qcr.FindByAuthorId(_authenticationContext.UserId,
                                        request.CollectionSortOrder,
                                        request.PageArgs.CalculateSkip(),
                                        request.PageArgs.PageSize + 1,
                                        cancellationToken);

        var hasNext = query.Count == request.PageArgs.PageSize + 1;

        var totalCount = await _qcr.CountByAuthorId(_authenticationContext.UserId, cancellationToken);

        return GenericResult<PagedResponse<GetCollectionResponse>>.Success(
            new(query.Take(request.PageArgs.PageSize).Select(e => e.ToGetCollectionResponse()).ToList(),
            hasNext,
            request.PageArgs.Page,
            request.PageArgs.PageSize)
            {
                TotalCount = totalCount,
                TotalPage = NumericCalcHelper.GetTotalPage(totalCount, request.PageArgs.PageSize)
            });
    }
}
