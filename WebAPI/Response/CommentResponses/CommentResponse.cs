using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.CommentResponses;

public class CommentResponse : IResourceRight<CommentResponse>
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ModificationDate { get; set; }
    public string? Content { get; set; }
    public AuthorResponse? Author { get; set; } = default!;
    public string ResourceRight { get; set; } = nameof(ResourceRights.Viewer);

    public CommentResponse SetResourceRight(int? requesterId)
    {
        if (Author != null && requesterId != null)
            ResourceRight = requesterId.Value == Author.Id ? nameof(ResourceRights.Owner) : nameof(ResourceRights.Viewer);

        return this;
    }
}
