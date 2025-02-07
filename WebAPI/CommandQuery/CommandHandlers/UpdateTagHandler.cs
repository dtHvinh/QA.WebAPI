using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateTagHandler(ITagRepository tagRepository) : ICommandHandler<UpdateTagCommand, GenericResult<GenericResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<GenericResponse>> Handle(
        UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var newTag = request.Tag.ToTag();

        _tagRepository.Update(newTag);

        var result = await _tagRepository.SaveChangesAsync(cancellationToken);

        return !result.IsSuccess
            ? GenericResult<GenericResponse>.Failure(result.Message)
            : GenericResult<GenericResponse>.Success("OK");
    }
}
