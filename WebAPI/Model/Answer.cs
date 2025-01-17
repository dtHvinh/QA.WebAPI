using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(QuestionId))]
public class Answer : IEntityWithTime<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(Question))]
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; } = default!;

    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }
    public AppUser? Author { get; set; } = default!;
    [Column(TypeName = "nvarchar(2000)")]
    public required string Content { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }

    public bool IsAccepted { get; set; } = false;

    public ICollection<AnswerReport> Reports { get; set; } = default!;
    public ICollection<AnswerUpvote> Upvotes { get; set; } = default!;
    public ICollection<AnswerDownvote> Downvotes { get; set; } = default!;
    public ICollection<AnswerComment> Comments { get; set; } = default!;
}
