namespace WebAPI.Response.CommunityResponses;

public record ChatRoomResponse(
    int Id,
    string Name,
    List<ChatMessageResponse> Messages);
