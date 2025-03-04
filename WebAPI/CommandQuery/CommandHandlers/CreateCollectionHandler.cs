using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
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

        if (result.IsSuccess)
        {
            _logger.Information("User Id {UserId} created collection with id {Id}", _authenticationContext.UserId, collection.Id);
        }

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success("Done")
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
