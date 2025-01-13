namespace WebAPI.Utilities.Options;

public class ApplicationProperties
{
    public required ActionRequirements ActionRequirements { get; set; }
    public required ReputationAcquirePerAction ReputationAcquirePerAction { get; set; }
}

public class ActionRequirements
{
    public int CreateTag { get; set; }
    public int UpdateTag { get; set; }
    public int DeleteTag { get; set; }
}

public class ReputationAcquirePerAction
{
    public int CreateQuestion { get; set; }
    public int AnswerQuestion { get; set; }
    public int UpvoteQuestion { get; set; }
    public int DownvoteQuestion { get; set; }
    public int UpvoteAnswer { get; set; }
    public int DownvoteAnswer { get; set; }
    public int AnswerAccepted { get; set; }
}
