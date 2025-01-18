using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteTagHandler(ITagRepository tagRepository)
    : ICommandHandler<DeleteTagCommand, GenericResult<DeleteTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<DeleteTagResponse>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        _tagRepository.DeleteTag(request.Id);

        var delTag = await _tagRepository.SaveChangesAsync(cancellationToken);

        if (!delTag.IsSuccess)
        {
            return GenericResult<DeleteTagResponse>.Failure(delTag.Message);
        }

        return GenericResult<DeleteTagResponse>.Success(new(request.Id));
    }
}
