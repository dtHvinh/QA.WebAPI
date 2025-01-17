using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Model;

public class AnswerComment : Comment
{
    [ForeignKey(nameof(Answer))]
    public Guid AnswerId { get; set; }
    public Answer? Answer { get; set; }
}
