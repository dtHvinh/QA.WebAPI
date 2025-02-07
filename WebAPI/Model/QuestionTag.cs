namespace WebAPI.Model;

public class QuestionTag
{
    public int QuestionId { get; set; }
    public Question Question { get; set; } = default!;
    public int TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
