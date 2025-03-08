using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCollectionHandler(
    ICollectionRepository qcr,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger) : ICommandHandler<CreateCollectionCommand, GenericResult<GenericResponse>>
{
    private readonly ICollectionRepository _questionCollectionRepository = qcr;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<GenericResponse>> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = request.Dto.ToCollection();

        collection.AuthorId = _authenticationContext.UserId;

        _questionCollectionRepository.Add(collection);

        var result = await _questionCollectionRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Created, collection);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success("Done")
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
