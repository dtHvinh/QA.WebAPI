using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AnswerId))]
public class AnswerReport : IEntity<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public AnswerReportKind Kind { get; set; }
    public required string Description { get; set; }

    [ForeignKey(nameof(Reporter))]
    public Guid ReporterId { get; set; }
    public AppUser? Reporter { get; set; } = default!;

    [ForeignKey(nameof(Answer))]
    public Guid AnswerId { get; set; }
    public Answer? Answer { get; set; } = default!;
}

public enum AnswerReportKind
{
    Inappropriate,
    NotRelevant,
    Incomplete,
    Other
}