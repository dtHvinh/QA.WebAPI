namespace WebAPI.Model;

public class QuestionVote : Vote
{
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; }
}
