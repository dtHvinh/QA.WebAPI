using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(UpvotedEntityId))]
public class Upvote : IEntity<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }

    public UpvoteType UpvoteType { get; set; }

    public Guid UpvotedEntityId { get; set; }
}

public enum UpvoteType
{
    Answer,
    Question
}