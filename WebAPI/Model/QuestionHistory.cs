using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class QuestionHistory : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public int AuthorId { get; set; }
    public AppUser? Author { get; set; }

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question Question { get; set; } = default!;

    public string QuestionHistoryType { get; set; } = default!;
    public string Comment { get; set; } = default!;
}

public static class QuestionHistoryType
{
    public const string Edit = "Edit";
    public const string Close = "Close";
    public const string Reopen = "Reopen";
    public const string AddComment = "Add Comment";
    public const string AddAnswer = "Add Answer";
    public const string AcceptAnswer = "Accept Answer";
}
