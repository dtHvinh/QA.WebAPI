using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCollectionLikeHandler(
    ICollectionLikeRepository collectionLikeRepository,
    ICollectionRepository collectionRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<CreateCollectionLikeCommand, GenericResult<TextResponse>>
{
    private readonly ICollectionLikeRepository _collectionLikeRepository = collectionLikeRepository;
    private readonly ICollectionRepository _collectionRepository = collectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(CreateCollectionLikeCommand request,
        CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.FindByIdAsync(request.CollectionId, cancellationToken);

        if (collection is null)
            return GenericResult<TextResponse>.Failure("Collection not found");

        _collectionLikeRepository.LikeCollection(collection, _authenticationContext.UserId);

        collection.LikeCount++;

        var res = await _collectionLikeRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Liked, collection);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("Done")
            : GenericResult<TextResponse>.Failure(res.Message);
    }
}