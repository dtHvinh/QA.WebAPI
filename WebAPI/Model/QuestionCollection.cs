namespace WebAPI.Model;

public class QuestionCollection
{
    public int QuestionId { get; set; }
    public int CollectionId { get; set; }
    public Question Question { get; set; } = default!;
    public Collection Collection { get; set; } = default!;
}
