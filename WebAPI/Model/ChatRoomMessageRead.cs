using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[PrimaryKey(nameof(UserId), nameof(ChatRoomMessageId))]
public class ChatRoomMessageRead : IKeylessEntity
{
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [ForeignKey(nameof(ChatRoomMessage))]
    public int ChatRoomMessageId { get; set; }
    public ChatRoomMessage? ChatRoomMessage { get; set; }

    public DateTime CreatedAt { get; set; }
}
