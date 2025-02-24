using MediatR;
using WebAPI.CommandQuery.Queries;
using WebAPI.Repositories.Base;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCollectionAndAddStatusHandler(
    ICollectionRepository questionCollectionRepository,
    AuthenticationContext authenticationContext)
    : IRequestHandler<GetUserCollectionAndAddStatusQuery,
        GenericResult<List<GetCollectionWithAddStatusResponse>>>
{
    private readonly ICollectionRepository _qcr = questionCollectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<List<GetCollectionWithAddStatusResponse>>> Handle(GetUserCollectionAndAddStatusQuery request, CancellationToken cancellationToken)
    {
        var collection = await _qcr.FindByAuthorId(_authenticationContext.UserId, Model.CollectionSortOrder.Newest, 0, 50, cancellationToken);

        var responses = collection.Select(e => new GetCollectionWithAddStatusResponse()
        {
            Id = e.Id,
            Name = e.Name,
            IsPublic = e.IsPublic,
        }).ToList();

        var addStatus = await _qcr.GetAddStatusAsync(collection.Select(e => e.Id).ToList(), request.QuestionId, cancellationToken);

        for (var i = 0; i < collection.Count; i++)
        {
            responses[i].IsAdded = addStatus[i];
        }

        return GenericResult<List<GetCollectionWithAddStatusResponse>>.Success(responses);
    }
}
