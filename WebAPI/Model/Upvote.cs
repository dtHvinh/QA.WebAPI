using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Upvote : IEntityWithTime<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }

    public string? UpvoteType { get; set; }
}

public enum UpvoteTypes
{
    Answer,
    Question
}