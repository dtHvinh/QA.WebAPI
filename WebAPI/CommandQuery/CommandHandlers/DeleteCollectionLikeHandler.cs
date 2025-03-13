using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteCollectionLikeHandler(
    ICollectionLikeRepository collectionLikeRepository,
    ICollectionRepository collectionRepository,
    AuthenticationContext authenticationContext)
    : ICommandHandler<DeleteCollectionLikeCommand, GenericResult<TextResponse>>
{
    private readonly ICollectionLikeRepository _collectionLikeRepository = collectionLikeRepository;
    private readonly ICollectionRepository _collectionRepository = collectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(DeleteCollectionLikeCommand request,
        CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.FindByIdAsync(request.CollectionId, cancellationToken);

        if (collection is null)
            return GenericResult<TextResponse>.Failure("Collection not found");

        await _collectionLikeRepository.UnlikeCollection(collection, _authenticationContext.UserId,
            cancellationToken);

        collection.LikeCount--;
        
        var res = await _collectionLikeRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("Done")
            : GenericResult<TextResponse>.Failure("Failed to unlike collection");
    }
}