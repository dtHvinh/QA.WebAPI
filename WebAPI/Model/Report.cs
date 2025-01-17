using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Report : IEntityWithTime<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [Column(TypeName = "nvarchar(150)")]
    public string Description { get; set; } = default!;
    public string? ReportType { get; set; }

    [ForeignKey(nameof(Reporter))]
    public Guid ReporterId { get; set; }
    public AppUser? Reporter { get; set; } = default!;
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