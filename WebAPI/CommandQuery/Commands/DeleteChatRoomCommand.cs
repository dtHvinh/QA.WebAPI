using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record DeleteChatRoomCommand(int CommunityId, int RoomId)
    : ICommand<GenericResult<TextResponse>>;
