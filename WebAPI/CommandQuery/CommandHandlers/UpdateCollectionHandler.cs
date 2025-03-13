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

public class UpdateCollectionHandler(ICollectionRepository repository, AuthenticationContext authenticationContext, Serilog.ILogger logger) : ICommandHandler<UpdateCollectionCommand, GenericResult<TextResponse>>
{
    private readonly ICollectionRepository _qcr = repository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(UpdateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _qcr.FindByIdAsync(request.Dto.Id, cancellationToken);

        if (collection == null)
            return GenericResult<TextResponse>.Failure("Not found");

        if (!_authenticationContext.IsResourceOwnedByUser(collection))
            return GenericResult<TextResponse>.Failure(EM.ACTION_REQUIRE_RESOURCE_OWNER);

        collection.IsPublic = request.Dto.IsPublic;
        collection.Name = request.Dto.Name;
        collection.Description = request.Dto.Description;
        _qcr.Update(collection);

        var res = await _qcr.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Updated, collection);

        return res.IsSuccess
        ? GenericResult<TextResponse>.Success("Updated")
        : GenericResult<TextResponse>.Failure(res.Message);
    }
}
