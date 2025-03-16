using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class ChatRoomMessage : IEntityWithTime<int>
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Message { get; set; } = default!;

    [ForeignKey(nameof(ChatRoom))]
    public int ChatRoomId { get; set; }
    public CommunityChatRoom ChatRoom { get; set; } = default!;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public AppUser User { get; set; } = default!;
}
