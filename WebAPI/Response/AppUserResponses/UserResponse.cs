using WebAPI.Response.ExternalLinkResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.AppUserResponses;

public class UserResponse : IResourceRight<int>
{
    public string Username { get; set; }
    public int Reputation { get; set; }
    public bool IsBanned { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; }
    public string ProfilePicture { get; set; } = default!;
    public string Bio { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public int QuestionCount { get; set; }
    public int AnswerCount { get; set; }
    public int CommentCount { get; set; }
    public int CollectionCount { get; set; }
    public List<ExternalLinkResponse> ExternalLinks { get; set; } = default!;
    public int AcceptedAnswerCount { get; set; }
    public int TotalUpvotes { get; set; }
    public string ResourceRight { get; set; } = nameof(ResourceRights.Viewer);

    public int SetResourceRight(int? requesterId)
    {
        throw new NotImplementedException();
    }
}
