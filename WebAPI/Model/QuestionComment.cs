using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Model;

public class QuestionComment : Comment
{
    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question? Question { get; set; } = default!;
}
