using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CommunityModHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<CommunityModRoleCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(CommunityModRoleCommand request, CancellationToken cancellationToken)
    {
        var getMember = await _communityRepository.GetMemberAsync(request.UserId, request.CommunityId, cancellationToken);

        if (getMember is null)
            return GenericResult<TextResponse>.Failure("User is not a member of this community.");

        if (!await _communityRepository.IsModerator(_authenticationContext.UserId, request.CommunityId, cancellationToken))
            return GenericResult<TextResponse>.Failure("You are not a moderator of this community.");

        if (request.IsGranting)
        {
            getMember.IsModerator = true;
        }
        else
        {
            if (getMember.IsOwner)
                return GenericResult<TextResponse>.Failure("You cannot remove the owner role.");

            if (getMember.IsModerator)
                getMember.IsModerator = false;
        }

        _communityRepository.Update(getMember);

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(
            level: res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error,
            userId: _authenticationContext.UserId,
            op: request.IsGranting ? LogOp.GrantCommunityMod : LogOp.RevokedCommunityMod,
            targetType: typeof(AppUser),
            targetId: request.UserId);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("Role updated successfully.")
            : GenericResult<TextResponse>.Failure("Failed to update role.");
    }
}
