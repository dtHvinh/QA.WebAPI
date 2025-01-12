namespace WebAPI.Utilities.Options;

public sealed class ApplicationProperties
{
    public required ActionRequirements ActionRequirements { get; set; }
    public required ReputationAcquirePerAction ReputationAcquirePerAction { get; set; }
}

public sealed class ActionRequirements
{
    public int CreateTagReputationRequirement { get; set; }
}

public sealed class ReputationAcquirePerAction
{
    public int CreateTag { get; set; }
    public int CreateQuestion { get; set; }
    public int AnswerQuestion { get; set; }
    public int UpvoteQuestion { get; set; }
    public int DownvoteQuestion { get; set; }
    public int UpvoteAnswer { get; set; }
    public int DownvoteAnswer { get; set; }
    public int AnswerAccepted { get; set; }
}
