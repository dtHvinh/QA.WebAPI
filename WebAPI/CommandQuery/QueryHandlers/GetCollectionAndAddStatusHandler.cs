using MediatR;
using WebAPI.CommandQuery.Queries;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCollectionAndAddStatusHandler(
    ICollectionRepository questionCollectionRepository,
    AuthenticationContext authenticationContext)
    : IRequestHandler<GetUserCollectionAndAddStatusQuery,
        GenericResult<PagedResponse<GetCollectionWithAddStatusResponse>>>
{
    private readonly ICollectionRepository _qcr = questionCollectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<PagedResponse<GetCollectionWithAddStatusResponse>>> Handle(GetUserCollectionAndAddStatusQuery request, CancellationToken cancellationToken)
    {
        var collection = await _qcr.FindWithAddStatusByAuthorId(_authenticationContext.UserId, request.QuestionId, request.PageArgs.CalculateSkip(), request.PageArgs.PageSize + 1, cancellationToken);

        var hasNext = collection.Count == request.PageArgs.PageSize + 1;

        return GenericResult<PagedResponse<GetCollectionWithAddStatusResponse>>.Success(
            new(collection.Select(e => new GetCollectionWithAddStatusResponse()
            {
                Id = e.Id,
                IsAdded = e.IsAdded,
                Name = e.Name,
                IsPublic = e.IsPublic
            }).Take(request.PageArgs.PageSize).ToList(),
            hasNext,
            request.PageArgs.PageIndex,
            request.PageArgs.PageSize));
    }
}
