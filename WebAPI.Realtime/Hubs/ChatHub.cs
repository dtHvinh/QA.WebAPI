using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Realtime.Hubs
{
    public class AbstractHub<TClient> : Hub<TClient>
        where TClient : class
    {
        public static string MapGroup(string roomId) => $"Room_{roomId}";
        public static string MapGroup(int roomId) => $"Room_{roomId}";

        protected virtual async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, MapGroup(roomId));
        }

        protected virtual async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, MapGroup(roomId));
        }
    }

    public class AbstractHub : Hub
    {
        public static string MapGroup(string roomId) => $"Room_{roomId}";
        public static string MapGroup(int roomId) => $"Room_{roomId}";

        protected virtual async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, MapGroup(roomId));
        }

        protected virtual async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, MapGroup(roomId));
        }
    }
}