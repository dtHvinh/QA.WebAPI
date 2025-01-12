using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class QuestionReport : IEntityWithTime<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public QuestionReportKind Kind { get; set; }
    [Column(TypeName = "nvarchar(150)")]
    public required string Description { get; set; }

    [ForeignKey(nameof(Reporter))]
    public Guid ReporterId { get; set; }
    public AppUser? Reporter { get; set; } = default!;

    [ForeignKey(nameof(Answer))]
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; } = default!;
}

public enum QuestionReportKind
{
    Inappropriate,
    WrongTag,
    Ambiguous,
    Other
}