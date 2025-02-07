using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class QuestionReport : Report, IKeylessEntityWithTime
{

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question? Question { get; set; } = default!;
}
