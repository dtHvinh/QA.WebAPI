namespace WebAPI.Model;

public class QuestionTag
{
    public int QuestionId { get; set; }
    public int TagId { get; set; }

    public Question Question { get; set; } = default!;
    public Tag Tag { get; set; } = default!;
}
