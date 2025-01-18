using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Upvote : IEntityWithTime<Guid>, IOwnedByUser<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }
    public AppUser? Author { get; set; }

    public string? UpvoteType { get; set; }
}

public enum UpvoteTypes
{
    Answer,
    Question
}