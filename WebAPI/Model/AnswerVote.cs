namespace WebAPI.Model;

public class AnswerVote : Vote
{
    public Guid AnswerId { get; set; }
    public Answer? Answer { get; set; }
}
