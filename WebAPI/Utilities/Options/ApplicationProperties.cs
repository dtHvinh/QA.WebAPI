namespace WebAPI.Utilities.Options;

public class ApplicationProperties
{
    public required ReputationRequirePerAction ActionRepRequirement { get; set; }
    public required ReputationAcquirePerAction ReputationAcquirePerAction { get; set; }
    public required ReputationRequireForRole ReputationRequireForRole { get; set; }
}

public class ReputationRequireForRole
{
    public int Moderator { get; set; }
}

public class ReputationRequirePerAction
{
    public int CreateQuestion { get; set; }
    public int Upvote { get; set; }
    public int Comment { get; set; }
    public int Downvote { get; set; }
    public int CreateTags { get; set; }
    public int ProtectQuestion { get; set; }
}

public class ReputationAcquirePerAction
{
    public int QuestionUpvoted { get; set; }
    public int QuestionDownvoted { get; set; }
    public int AnswerUpvoted { get; set; }
    public int AnswerDownvoted { get; set; }
    public int DownvoteAnswer { get; set; }
    public int DownvoteQuestion { get; set; }
    public int AnswerAccepted { get; set; }
}