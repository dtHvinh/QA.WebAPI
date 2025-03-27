using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class RemoveCommunityMemberHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<RemoveCommunityMemberCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(RemoveCommunityMemberCommand request, CancellationToken cancellationToken)
    {
        if (!await _communityRepository.IsModerator(_authenticationContext.UserId, request.CommunityId, cancellationToken))
            return GenericResult<TextResponse>.Failure("You are not a moderator of this community.");

        if (_authenticationContext.UserId == request.UserId)
            return GenericResult<TextResponse>.Failure("You cannot remove yourself from the community.");

        var member = await _communityRepository.GetMemberAsync(request.UserId, request.CommunityId, cancellationToken);

        if (member is null)
            return GenericResult<TextResponse>.Failure("User is not a member of this community.");

        if (await _communityRepository.IsModerator(request.UserId, request.CommunityId, cancellationToken))
            return GenericResult<TextResponse>.Failure("You are not allowed to remove a moderator from this community.");

        _communityRepository.RemoveMember(member);

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId, LogOp.Removed, member);

        return res.IsSuccess
                ? GenericResult<TextResponse>.Success("User has been removed from the community.")
                : GenericResult<TextResponse>.Failure("Failed to remove user from the community.");
    }
}
