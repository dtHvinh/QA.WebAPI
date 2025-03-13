using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteCollectionHandler(
    ICollectionRepository questionCollectionRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<DeleteCollectionCommand, GenericResult<TextResponse>>
{
    private readonly ICollectionRepository _qcRepository = questionCollectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _qcRepository.FindByIdAsync(request.Id, cancellationToken);
        if (collection == null)
        {
            return GenericResult<TextResponse>.Failure("Collection not found.");
        }

        if (!_authenticationContext.IsResourceOwnedByUser(collection))
        {
            return GenericResult<TextResponse>.Failure(EM.ACTION_REQUIRE_RESOURCE_OWNER);
        }

        _qcRepository.Remove(collection);

        var res = await _qcRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Deleted, collection);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("Collection deleted successfully.")
            : GenericResult<TextResponse>.Failure("Failed to delete question collection.");
    }
}