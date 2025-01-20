using WebAPI.Response.AppUserResponses;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Response.CommentResponses;
using WebAPI.Response.TagResponses;

namespace WebAPI.Response.QuestionResponses;

public class GetQuestionResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public AuthorResponse? Author { get; set; } = default!;

    public bool IsDuplicate { get; set; } = false;
    public bool IsClosed { get; set; } = false;
    public bool IsDraft { get; set; } = false;

    public int ViewCount { get; set; }
    public int AnswerCount { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }

    public ICollection<TagResponse> Tags { get; set; } = default!;
    public ICollection<AnswerResponse> Answers { get; set; } = default!;
    public ICollection<CommentResponse> Comments { get; set; } = default!;
}

