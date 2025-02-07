namespace WebAPI.Model;

public class AnswerVote : Vote
{
    public int AnswerId { get; set; }
    public Answer? Answer { get; set; }
}
