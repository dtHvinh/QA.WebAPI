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
    public int CreateTag { get; set; }
    public int UpdateTag { get; set; }
    public int DeleteTag { get; set; }
    public int CloseQuestion { get; set; }
    public int CommentToQuestion { get; set; }
    public int AnswerQuestion { get; set; }
    public int UpvoteAndDownvote { get; set; }
    public int EditQuestion { get; set; }
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
    public int MarkAsSpam { get; set; }
}
