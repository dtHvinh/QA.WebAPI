using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
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
        var questionCollection = await _qcRepository.FindByIdAsync(request.Id, cancellationToken);
        if (questionCollection == null)
        {
            return GenericResult<GenericResponse>.Failure("Collection not found.");
        }

        if (!_authenticationContext.IsResourceOwnedByUser(questionCollection))
        {
            return GenericResult<GenericResponse>.Failure(EM.ACTION_REQUIRE_RESOURCE_OWNER);
        }

        _qcRepository.Remove(questionCollection);

        var res = await _qcRepository.SaveChangesAsync(cancellationToken);

        if (res.IsSuccess)
        {
            _logger.Information("Collection {CollectionId} deleted by {UserId}", questionCollection.Id, _authenticationContext.UserId);
        }

        return res.IsSuccess
            ? GenericResult<GenericResponse>.Success("Collection deleted successfully.")
            : GenericResult<GenericResponse>.Failure("Failed to delete question collection.");
    }
}