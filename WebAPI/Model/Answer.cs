using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(QuestionId))]
public class Answer : IEntityWithTime<int>, IOwnedByUser<int>, ISoftDeleteEntity, IScorable
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTime.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }

    public required string Content { get; set; }
    public int Score { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsAccepted { get; set; } = false;

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question? Question { get; set; } = default!;

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; } = default!;


    public ICollection<AnswerVote> Votes { get; set; } = default!;
    public ICollection<AnswerComment> Comments { get; set; } = default!;
}
