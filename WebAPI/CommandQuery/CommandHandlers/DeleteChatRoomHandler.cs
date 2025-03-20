using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteChatRoomHandler(ICommunityRepository communityRepository, AuthenticationContext authenticationContext)
    : ICommandHandler<DeleteChatRoomCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(DeleteChatRoomCommand request, CancellationToken cancellationToken)
    {
        var isModerator = await _communityRepository.IsModerator(_authenticationContext.UserId, request.CommunityId, cancellationToken);

        if (!isModerator)
        {
            return GenericResult<TextResponse>.Failure("You are not a moderator of this community.");
        }

        var chatRoom = await _communityRepository.GetRoom(request.RoomId, cancellationToken);

        if (chatRoom == null)
        {
            return GenericResult<TextResponse>.Failure("Chat room not found.");
        }

        _communityRepository.DeleteChatRoom(chatRoom);

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success(new TextResponse("Chat room deleted."))
            : GenericResult<TextResponse>.Failure("Failed to delete chat room.");
    }
}
