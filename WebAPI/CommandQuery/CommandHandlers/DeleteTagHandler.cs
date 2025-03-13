using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteTagHandler(ITagRepository tagRepository, AuthenticationContext authenticationContext, Serilog.ILogger logger)
    : ICommandHandler<DeleteTagCommand, GenericResult<DeleteTagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<DeleteTagResponse>> Handle(DeleteTagCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsModerator())
            return GenericResult<DeleteTagResponse>.Failure(string.Format(Constants.EM.ROLE_NOT_MEET_REQ,
                nameof(Constants.Roles.Moderator)));

        var tagToDel = await _tagRepository.FindFirstAsync(e => e.Id.Equals(request.Id), cancellationToken);

        if (tagToDel == null)
            return GenericResult<DeleteTagResponse>.Failure("Not found");

        _tagRepository.Remove(tagToDel);

        var delTag = await _tagRepository.SaveChangesAsync(cancellationToken);

        if (!delTag.IsSuccess)
        {
            return GenericResult<DeleteTagResponse>.Failure(delTag.Message);
        }

        _logger.ModeratorNoEnityOwnerAction(delTag.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogModeratorOp.Delete, tagToDel);

        return GenericResult<DeleteTagResponse>.Success(new(request.Id));
    }
}