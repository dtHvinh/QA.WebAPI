namespace WebAPI.Model;

public class QuestionTag
{
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = default!;
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
