using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateTagHandler(ITagRepository tagRepository) : ICommandHandler<CreateTagCommand, OperationResult<CreateTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<OperationResult<CreateTagResponse>> Handle(
        CreateTagCommand request, CancellationToken cancellationToken)
    {
        var newTag = request.Tag.ToTag();

        var createTag = await _tagRepository.CreateTagAsync(newTag, cancellationToken);
        if (!createTag.IsSuccess)
        {
            return OperationResult<CreateTagResponse>.Failure(createTag.Message);
        }

        return OperationResult<CreateTagResponse>.Success(newTag.ToCreateTagResponse());
    }
}
