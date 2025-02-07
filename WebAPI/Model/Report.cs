using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AuthorId))]
public class Report : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    [Column(TypeName = "nvarchar(150)")]
    public string Description { get; set; } = default!;
    public string? ReportType { get; set; }

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public AppUser? Author { get; set; } = default!;
}

public enum QuestionReportKind
{
    Inappropriate,
    WrongTag,
    Ambiguous,
    Other
}

public enum AnswerReportKind
{
    Inappropriate,
    NotRelevant,
    Incomplete,
    Other
}

public enum ReportTypes
{
    Question,
    Answer
}