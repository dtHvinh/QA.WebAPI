using WebAPI.Utilities.Response.AppUserResponses;

namespace WebAPI.Utilities.Response.CommentResponses;

public class CommentResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Content { get; set; }
    public AuthorResponse Author { get; set; } = default!;
}
