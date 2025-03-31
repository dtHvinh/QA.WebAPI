using WebAPI.Response.CommunityResponses;

namespace WebAPI.Realtime;

public interface IRoomChatClient
{
    Task ReceiveMessage(ChatMessageResponse message);
    Task SomeOneStopTyping(string username, string userId);
    Task SomeOneStartTyping(string username, string userId);
}
