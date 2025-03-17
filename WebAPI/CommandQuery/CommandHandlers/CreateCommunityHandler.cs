using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCommunityHandler(
    ICommunityRepository communityRepository,
    IUserRepository userRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<CreateCommunityCommand, GenericResult<CreateCommunityResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<CreateCommunityResponse>> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
        var communityOwner = await _userRepository.FindUserByIdAsync(_authenticationContext.UserId, cancellationToken);

        if (communityOwner is null)
            return GenericResult<CreateCommunityResponse>.Failure("User not found");

        var defaultGlobalChatRoom = new CommunityChatRoom()
        {
            Name = "global",
        };

        var comunity = new Community()
        {
            Description = request.CreateDto.Description,
            IsPrivate = request.CreateDto.IsPrivate,
            Name = request.CreateDto.Name,
            Members = [new() {
                User = communityOwner,
                IsOwner = true,
            }],
            Rooms = [defaultGlobalChatRoom],
        };

        _communityRepository.CreateCommunity(comunity);

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        if (res.IsSuccess)
            _logger.UserAction(Serilog.Events.LogEventLevel.Information, _authenticationContext.UserId, LogOp.Created, comunity);

        return res.IsSuccess
            ? GenericResult<CreateCommunityResponse>.Success(new CreateCommunityResponse(comunity.Name))
            : GenericResult<CreateCommunityResponse>.Failure(res.Message);
    }
}
