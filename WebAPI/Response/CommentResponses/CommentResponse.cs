using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.CommentResponses;

public class CommentResponse : IResourceRight<CommentResponse>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Content { get; set; }
    public AuthorResponse? Author { get; set; } = default!;
    public string ResourceRight { get; set; } = nameof(ResourceRights.Owner);

    public CommentResponse SetResourceRight(Guid? requesterId)
    {
        if (Author != null && requesterId != null)
            ResourceRight = requesterId.Value == Author.Id ? nameof(ResourceRights.Owner) : nameof(ResourceRights.Viewer);

        return this;
    }
}
