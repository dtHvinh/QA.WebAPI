using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class LeaveCommunityHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<LeaveCommunityCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(LeaveCommunityCommand request, CancellationToken cancellationToken)
    {
        var member = await _communityRepository.GetMemberAsync(
             _authenticationContext.UserId, request.CommunityId, cancellationToken);

        if (member is null)
            return GenericResult<TextResponse>.Failure("You are not a member of this community.");

        _communityRepository.RemoveMember(member);

        member.Community.MemberCount -= 1;

        _communityRepository.Update(member.Community);

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error,
            _authenticationContext.UserId, LogOp.Left, typeof(Community), member.CommunityId);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("You have left the community.")
            : GenericResult<TextResponse>.Failure("Failed to leave the community.");
    }
}
