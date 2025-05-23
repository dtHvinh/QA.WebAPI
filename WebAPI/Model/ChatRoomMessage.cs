﻿using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class ChatRoomMessage : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }

    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }

    public string Message { get; set; } = default!;

    [ForeignKey(nameof(ChatRoom))]
    public int ChatRoomId { get; set; }
    public CommunityChatRoom ChatRoom { get; set; } = default!;

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; } = default!;
}
