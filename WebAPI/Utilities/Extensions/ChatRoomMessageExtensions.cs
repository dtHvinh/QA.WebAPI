using Riok.Mapperly.Abstractions;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.CommunityResponses;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class ChatRoomMessageExtensions
{
    public static partial ChatRoomMessage ToChatRoomMessage(this ChatRequestDto source);

    public static ChatRoomMessage WithAuthor(this ChatRoomMessage message, ApplicationUser author)
    {
        message.Author = author;
        return message;
    }

    public static ChatRoomMessage WithAuthor(this ChatRoomMessage message, int authorId)
    {
        message.AuthorId = authorId;
        return message;
    }

    public static ChatRoomMessage WithChatRoom(this ChatRoomMessage message, CommunityChatRoom chatRoom)
    {
        message.ChatRoom = chatRoom;
        return message;
    }

    public static ChatMessageResponse ToResponseWithAuthor(this ChatRoomMessage message)
    {
        if (message.Id == default)
            throw new InvalidOperationException("ChatRoomMessage Id is not set");

        ArgumentNullException.ThrowIfNull(message.Author, nameof(message.Author));

        return ChatMessageResponse.From(
            message.Id, message.Message, message.CreationDate, message.ModificationDate, message.Author.ToAuthorResponse()!);
    }
}
