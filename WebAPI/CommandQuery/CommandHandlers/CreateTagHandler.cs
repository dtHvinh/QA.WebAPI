using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateTagHandler(ITagRepository tagRepository) : ICommandHandler<CreateTagCommand, GenericResult<CreateTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<CreateTagResponse>> Handle(
        CreateTagCommand request, CancellationToken cancellationToken)
    {
        var newTag = request.Tag.ToTag();

        _tagRepository.CreateTag(newTag);

        var createTag = await _tagRepository.SaveChangesAsync(cancellationToken);
        if (!createTag.IsSuccess)
        {
            return GenericResult<CreateTagResponse>.Failure(createTag.Message);
        }

        return GenericResult<CreateTagResponse>.Success(newTag.ToCreateTagResponse());
    }
}
