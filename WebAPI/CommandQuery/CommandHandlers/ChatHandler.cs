using Microsoft.AspNetCore.SignalR;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Realtime;
using WebAPI.Realtime.Hubs;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class ChatHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext,
    IHubContext<RoomChatHub> hubContext,
    IUserRepository userRepository)
    : ICommandHandler<ChatCommand, GenericResult<ChatMessageResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly IHubContext<RoomChatHub> _hubContext = hubContext;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<GenericResult<ChatMessageResponse>> Handle(
        ChatCommand request, CancellationToken cancellationToken)
    {
        var chatMessage = request.Dto.ToChatRoomMessage()
            .WithAuthor(_authenticationContext.UserId);

        _communityRepository.CreateChatMessage(chatMessage);

        var author = await _userRepository.FindUserByIdAsync(_authenticationContext.UserId, cancellationToken);

        if (author == null)
        {
            return GenericResult<ChatMessageResponse>.Failure("Author not found");
        }

        chatMessage.Author = author;

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        var chatResponse = chatMessage.ToResponseWithAuthor();

        if (res.IsSuccess)
        {
            await _hubContext.Clients
                .Group(AbstractHub<IRoomChatClient>.MapGroup(request.Dto.ChatRoomId))
                .SendAsync(
                method: "ReceiveMessage",
                arg1: chatResponse,
                cancellationToken: cancellationToken);
        }

        return res.IsSuccess
            ? GenericResult<ChatMessageResponse>.Success(chatResponse)
            : GenericResult<ChatMessageResponse>.Failure("Fail to send message");
    }
}
