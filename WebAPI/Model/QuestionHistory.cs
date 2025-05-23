using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class QuestionHistory : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }

    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }

    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question Question { get; set; } = default!;

    public int QuestionHistoryTypeId { get; set; }
    public QuestionHistoryType QuestionHistoryType { get; set; } = default!;

    public string Comment { get; set; } = default!;
}
