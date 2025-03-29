using WebAPI.Response.AppUserResponses;

namespace WebAPI.Response.CommunityResponses;

public record ChatMessageResponse(
    int Id,
    string Message,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    AuthorResponse Author
);
