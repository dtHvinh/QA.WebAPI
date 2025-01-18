namespace WebAPI.Model;

public class QuestionUpvote : Upvote
{
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; }
}
