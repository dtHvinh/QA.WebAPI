using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateChatRoomHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<CreateChatRoomCommand, GenericResult<CreateChatRoomResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<CreateChatRoomResponse>> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
    {
        var chatRoom = new CommunityChatRoom()
        {
            CommunityId = request.CreateDto.CommunityId,
            Name = request.CreateDto.Name
        };

        _communityRepository.CreateChatRoom(chatRoom);

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId,
            LogOp.Created, chatRoom);

        return res.IsSuccess
            ? GenericResult<CreateChatRoomResponse>.Success(new CreateChatRoomResponse(chatRoom.Id))
            : GenericResult<CreateChatRoomResponse>.Failure(res.Message);
    }
}
