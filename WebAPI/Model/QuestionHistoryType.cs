using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class QuestionHistoryType : IEntity<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<QuestionHistory> Histories { get; set; } = default!;
}

public static class QuestionHistoryTypes
{
    public const string FlagDuplicate = "Flag Duplicate";
    public const string RemoveDuplicateFlag = "Remove Duplicate Flag";
    public const string Edit = "Edit";
    public const string Close = "Close Question";
    public const string Reopen = "Re-open Question";
    public const string AddComment = "Add Comment";
    public const string AddAnswer = "Add Answer";
    public const string AcceptAnswer = "Accept Answer";
}

/**
 * 
 *     
    public const string FlagDuplicate = "Flag Duplicate";
    public const string RemoveDuplicateFlag = "Remove Duplicate Flag";
    public const string Edit = "Edit";
    public const string Close = "Close Question";
    public const string Reopen = "Re-open Question";
    public const string AddComment = "Add Comment";
    public const string AddAnswer = "Add Answer";
    public const string AcceptAnswer = "Accept Answer";
 */
