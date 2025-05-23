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

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }
}
