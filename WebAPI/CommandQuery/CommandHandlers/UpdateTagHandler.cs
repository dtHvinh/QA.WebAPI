using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateTagHandler(ITagRepository tagRepository) : ICommandHandler<UpdateTagCommand, OperationResult<UpdateTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<OperationResult<UpdateTagResponse>> Handle(
        UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var newTag = request.Tag.ToTag();

        _tagRepository.Update(newTag);

        var result = await _tagRepository.SaveChangeAsync(cancellationToken);

        return !result.IsSuccess
            ? OperationResult<UpdateTagResponse>.Failure(result.Message)
            : OperationResult<UpdateTagResponse>.Success(
                UpdateTagResponse.Create(newTag.Name, newTag.Description));
    }
}
