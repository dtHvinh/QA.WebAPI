using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.AsnwerResponses;

public class AnswerResponse : IResourceRight<AnswerResponse>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public AuthorResponse? Author { get; set; } = default!;
    public required string Content { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }
    public bool IsAccepted { get; set; }
    public string ResourceRight { get; set; } = nameof(ResourceRights.Viewer);

    public AnswerResponse SetResourceRight(Guid? requesterId)
    {
        if (requesterId != null && Author != null && requesterId == Author.Id)
            ResourceRight = nameof(ResourceRights.Owner);

        return this;
    }
}
