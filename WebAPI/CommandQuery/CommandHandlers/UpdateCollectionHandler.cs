using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateCollectionHandler(IQuestionCollectionRepository repository) : ICommandHandler<UpdateCollectionCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionCollectionRepository _qcr = repository;

    public async Task<GenericResult<GenericResponse>> Handle(UpdateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _qcr.FindByIdAsync(request.Dto.Id, cancellationToken);

        if (collection == null)
        {
            return GenericResult<GenericResponse>.Failure("Not found");
        }

        collection.IsPublic = request.Dto.IsPublic;
        collection.Name = request.Dto.Name;
        collection.Description = request.Dto.Description;

        _qcr.Update(collection);

        var res = await _qcr.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
        ? GenericResult<GenericResponse>.Success("Updated")
        : GenericResult<GenericResponse>.Failure(res.Message);
    }
}
