namespace WebAPI.Model;

public class QuestionVote : Vote
{
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}
