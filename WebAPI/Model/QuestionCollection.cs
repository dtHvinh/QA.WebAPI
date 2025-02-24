namespace WebAPI.Model;

public class QuestionCollection
{
    public int QuestionId { get; set; }
    public Question Question { get; set; } = default!;
    public int CollectionId { get; set; }
    public Collection Collection { get; set; } = default!;
}
