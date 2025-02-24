using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCollectionHandler(ICollectionRepository qcr, AuthenticationContext authenticationContext) : ICommandHandler<CreateCollectionCommand, GenericResult<GenericResponse>>
{
    private readonly ICollectionRepository _questionCollectionRepository = qcr;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = request.Dto.ToCollection();

        collection.AuthorId = _authenticationContext.UserId;

        _questionCollectionRepository.Add(collection);

        var result = await _questionCollectionRepository.SaveChangesAsync();

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success("Done")
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
