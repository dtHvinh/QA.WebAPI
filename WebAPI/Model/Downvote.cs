using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(DownvotedEntityId))]
public class Downvote : IEntityWithTime<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }

    public DownvoteType DownvoteType { get; set; }

    public Guid DownvotedEntityId { get; set; }
}

public enum DownvoteType
{
    Answer,
    Question
}