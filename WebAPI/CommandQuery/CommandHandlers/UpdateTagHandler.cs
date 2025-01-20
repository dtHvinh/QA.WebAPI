using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateTagHandler(ITagRepository tagRepository) : ICommandHandler<UpdateTagCommand, GenericResult<UpdateTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<UpdateTagResponse>> Handle(
        UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var newTag = request.Tag.ToTag();

        _tagRepository.Update(newTag);

        var result = await _tagRepository.SaveChangesAsync(cancellationToken);

        return !result.IsSuccess
            ? GenericResult<UpdateTagResponse>.Failure(result.Message)
            : GenericResult<UpdateTagResponse>.Success(
                UpdateTagResponse.Create(newTag.Name, newTag.Description));
    }
}
