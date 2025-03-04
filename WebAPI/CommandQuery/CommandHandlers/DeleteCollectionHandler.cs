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
    : ICommandHandler<DeleteCollectionCommand, GenericResult<GenericResponse>>
{
    private readonly ICollectionRepository _qcRepository = questionCollectionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<GenericResponse>> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _qcRepository.FindByIdAsync(request.Id, cancellationToken);
        if (collection == null)
        {
            return GenericResult<GenericResponse>.Failure("Collection not found.");
        }

        if (!_authenticationContext.IsResourceOwnedByUser(collection))
        {
            return GenericResult<GenericResponse>.Failure(EM.ACTION_REQUIRE_RESOURCE_OWNER);
        }

        _qcRepository.Remove(collection);

        var res = await _qcRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Deleted, collection);

        return res.IsSuccess
            ? GenericResult<GenericResponse>.Success("Collection deleted successfully.")
            : GenericResult<GenericResponse>.Failure("Failed to delete question collection.");
    }
}