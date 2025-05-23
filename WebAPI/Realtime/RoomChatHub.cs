using Microsoft.AspNetCore.SignalR;
using WebAPI.Realtime.Hubs;

namespace WebAPI.Realtime;

public class RoomChatHub(Serilog.ILogger logger) : AbstractHub
{
    private readonly Serilog.ILogger _logger = logger;

    public override Task OnConnectedAsync()
    {
        _logger.Information("----------Client connected: {ConnectionId}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public Task StartTyping(string username, string userId, string roomId)
    {
        _logger.Information("----------User start typing");
        return Clients.Group(MapGroup(roomId)).SendAsync("SomeOneStartTyping", username, userId);
    }

    public Task StopTyping(string username, string userId, string roomId)
    {
        _logger.Information("----------User stop typing");
        return Clients.Group(MapGroup(roomId)).SendAsync("SomeOneStopTyping", username, userId);
    }

    public new Task JoinRoom(string roomId)
    {
        _logger.Information("----------User join a room");
        return base.JoinRoom(roomId);
    }

    public new Task LeaveRoom(string roomId)
    {
        _logger.Information("----------User leave a room");
        return base.LeaveRoom(roomId);
    }
}
