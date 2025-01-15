using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Response.TagResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteTagHandler(ITagRepository tagRepository)
    : ICommandHandler<DeleteTagCommand, OperationResult<DeleteTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<OperationResult<DeleteTagResponse>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        _tagRepository.DeleteTag(request.Id);

        var delTag = await _tagRepository.SaveChangesAsync(cancellationToken);

        if (!delTag.IsSuccess)
        {
            return OperationResult<DeleteTagResponse>.Failure(delTag.Message);
        }

        return OperationResult<DeleteTagResponse>.Success(new(request.Id));
    }
}
