using WebAPI.Response.AppUserResponses;

namespace WebAPI.Response.CommunityResponses;

public class ChatMessageResponse
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public AuthorResponse Author { get; set; }

    public static ChatMessageResponse From(int id, string message, DateTime createdAt, DateTime updatedAt, AuthorResponse author)
    {
        return new ChatMessageResponse
        {
            Id = id,
            Message = message,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Author = author
        };
    }
}

public class ChatMessageResponseWithRoomId : ChatMessageResponse
{
    public int ChatRoomId { get; set; }
}