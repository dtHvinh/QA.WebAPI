using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class JoinCommunityHandler(
    ICommunityRepository communityRepository,
    IUserRepository userRepository,
    AuthenticationContext authenticationContext,
    ICacheService cacheService,
    Serilog.ILogger logger)
    : ICommandHandler<JoinCommunityCommand, GenericResult<TextResponse>>,
    ICommandHandler<JoinCommunityViaInvitationLinkCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly ICacheService _cacheService = cacheService;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(JoinCommunityCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserByIdAsync(_authenticationContext.UserId, cancellationToken);

        if (user is null)
            return GenericResult<TextResponse>.Failure("User not found");

        var community = await _communityRepository.FindFirstAsync(e => e.Id == request.CommunityId, cancellationToken);

        if (community is null)
            return GenericResult<TextResponse>.Failure("Community not found");

        var jResult = await _communityRepository.JoinCommunity(user, community, cancellationToken);

        if (jResult)
        {
            community.MemberCount++;
            _communityRepository.Update(community);
        }
        else
            return GenericResult<TextResponse>.Failure("Already a member of this community");

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess ?
            Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId,
            LogOp.Joined, community);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("Successfully joined community")
            : GenericResult<TextResponse>.Success("Failed to join community");
    }

    public async Task<GenericResult<TextResponse>> Handle(JoinCommunityViaInvitationLinkCommand request, CancellationToken cancellationToken)
    {
        var communityId = await _cacheService.GetCommunityIdFromInvLink(request.InvitationLink, cancellationToken);

        if (communityId is null)
            return GenericResult<TextResponse>.Failure("Invalid invitation link");

        var user = await _userRepository.FindUserByIdAsync(_authenticationContext.UserId, cancellationToken);

        if (user is null)
            return GenericResult<TextResponse>.Failure("User not found");

        var community = await _communityRepository.FindFirstAsync(e => e.Id == communityId, cancellationToken);

        if (community is null)
            return GenericResult<TextResponse>.Failure("Community not found");

        var jResult = await _communityRepository.JoinCommunity(user, community, cancellationToken);

        if (jResult)
        {
            community.MemberCount++;
            _communityRepository.Update(community);
        }
        else
            return GenericResult<TextResponse>.Failure("Already a member of this community");

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess ?
            Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId,
            LogOp.Joined, community);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success(community.Name)
            : GenericResult<TextResponse>.Success("Failed to join community");
    }
}
