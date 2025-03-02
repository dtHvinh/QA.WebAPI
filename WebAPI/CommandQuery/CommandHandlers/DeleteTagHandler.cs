using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteTagHandler(ITagRepository tagRepository, AuthenticationContext authenticationContext)
    : ICommandHandler<DeleteTagCommand, GenericResult<DeleteTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<DeleteTagResponse>> Handle(DeleteTagCommand request,
        CancellationToken cancellationToken)
    {
        if (!_authenticationContext.IsModerator())
            return GenericResult<DeleteTagResponse>.Failure(string.Format(Constants.EM.ROLE_NOT_MEET_REQ,
                nameof(Constants.Roles.Moderator)));

        _tagRepository.DeleteTag(request.Id);

        var delTag = await _tagRepository.SaveChangesAsync(cancellationToken);

        if (!delTag.IsSuccess)
        {
            return GenericResult<DeleteTagResponse>.Failure(delTag.Message);
        }

        return GenericResult<DeleteTagResponse>.Success(new(request.Id));
    }
}