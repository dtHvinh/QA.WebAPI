using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCollectionLikeHandler(
    ICollectionLikeRepository collectionLikeRepository,
    ICollectionRepository collectionRepository,
    AuthenticationContext authenticationContext)
    : ICommandHandler<CreateCollectionLikeCommand, GenericResult<GenericResponse>>
{
    private readonly ICollectionLikeRepository _collectionLikeRepository = collectionLikeRepository;
    private readonly ICollectionRepository _collectionRepository = collectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(CreateCollectionLikeCommand request,
        CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.FindByIdAsync(request.CollectionId, cancellationToken);

        if (collection is null)
            return GenericResult<GenericResponse>.Failure("Collection not found");

        _collectionLikeRepository.LikeCollection(collection, _authenticationContext.UserId);

        collection.LikeCount++;

        var res = await _collectionLikeRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? GenericResult<GenericResponse>.Success("Done")
            : GenericResult<GenericResponse>.Failure(res.Message);
    }
}