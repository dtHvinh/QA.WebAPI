using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCollectionLikeHandler(
    ICollectionLikeRepository collectionLikeRepository,
    ICollectionRepository collectionRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<CreateCollectionLikeCommand, GenericResult<GenericResponse>>
{
    private readonly ICollectionLikeRepository _collectionLikeRepository = collectionLikeRepository;
    private readonly ICollectionRepository _collectionRepository = collectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<GenericResponse>> Handle(CreateCollectionLikeCommand request,
        CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.FindByIdAsync(request.CollectionId, cancellationToken);

        if (collection is null)
            return GenericResult<GenericResponse>.Failure("Collection not found");

        _collectionLikeRepository.LikeCollection(collection, _authenticationContext.UserId);

        collection.LikeCount++;

        var res = await _collectionLikeRepository.SaveChangesAsync(cancellationToken);

        if (res.IsSuccess)
        {
            _logger.Information(
                "User {UserId} liked collection {CollectionId}",
                _authenticationContext.UserId,
                request.CollectionId
            );
        }

        return res.IsSuccess
            ? GenericResult<GenericResponse>.Success("Done")
            : GenericResult<GenericResponse>.Failure(res.Message);
    }
}