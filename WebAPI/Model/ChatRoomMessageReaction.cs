using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Keyless]
public class ChatRoomMessageReaction : IKeylessEntityWithTime, IOwnedByUser<int>
{
    [ForeignKey(nameof(ChatMessage))]
    public int ChatMessageId { get; set; }
    public ChatRoomMessage? ChatMessage { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public AppUser? Author { get; set; }
}
