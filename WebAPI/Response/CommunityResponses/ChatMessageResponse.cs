using WebAPI.Response.AppUserResponses;

namespace WebAPI.Response.CommunityResponses;

public class ChatMessageResponse
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ModificationDate { get; set; }
    public AuthorResponse Author { get; set; }

    public static ChatMessageResponse From(int id, string message, DateTimeOffset createdAt, DateTimeOffset updatedAt, AuthorResponse author)
    {
        return new ChatMessageResponse
        {
            Id = id,
            Message = message,
            CreationDate = createdAt,
            ModificationDate = updatedAt,
            Author = author
        };
    }
}

public class ChatMessageResponseWithRoomId : ChatMessageResponse
{
    public int ChatRoomId { get; set; }
}