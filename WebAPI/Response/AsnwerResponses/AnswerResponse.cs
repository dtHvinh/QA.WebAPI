using WebAPI.Response.AppUserResponses;

namespace WebAPI.Response.AsnwerResponses;

public class AnswerResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public AuthorResponse? Author { get; set; } = default!;
    public required string Content { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }
    public bool IsAccepted { get; set; }
}
