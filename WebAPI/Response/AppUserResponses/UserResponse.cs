using WebAPI.Response.ExternalLinkResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.AppUserResponses;

public class UserResponse : IResourceRight<int>
{
    public string Username { get; set; } = default!;
    public int Reputation { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }
    public string? ProfilePicture { get; set; } = default!;
    public bool IsDeleted { get; set; } = false;
    public int QuestionCount { get; set; }
    public int AnswerCount { get; set; }
    public int CommentCount { get; set; }
    public int CollectionCount { get; set; }
    public List<ExternalLinkResponse> ExternalLinks { get; set; } = default!;
    public int AcceptedAnswerCount { get; set; }
    public int TotalScore { get; set; }
    public string ResourceRight { get; set; } = nameof(ResourceRights.Viewer);

    public int SetResourceRight(int? requesterId)
    {
        throw new NotImplementedException();
    }
}
