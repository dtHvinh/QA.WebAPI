using WebAPI.Response.AppUserResponses;

namespace WebAPI.Response.CommentResponses;

public class CommentResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Content { get; set; }
    public AuthorResponse? Author { get; set; } = default!;
}
