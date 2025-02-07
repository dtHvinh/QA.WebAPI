using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class AnswerReport : Report, IKeylessEntityWithTime
{
    [ForeignKey(nameof(Answer))]
    public int? AnswerId { get; set; }
    public Answer? Answer { get; set; } = default!;
}
