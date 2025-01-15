using WebAPI.Utilities.Response.AppUserResponses;
using WebAPI.Utilities.Response.AsnwerResponses;
using WebAPI.Utilities.Response.CommentResponses;
using WebAPI.Utilities.Response.TagResponses;

namespace WebAPI.Utilities.Response.QuestionResponses;

public class GetQuestionResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }

    public AuthorResponse? Author { get; set; } = default!;

    public bool IsDuplicate { get; set; } = false;
    public bool IsClosed { get; set; } = false;
    public bool IsDraft { get; set; } = false;

    public int ViewCount { get; set; }
    public int AnswerCount { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }

    public List<TagResponse> Tags { get; set; }
    public ICollection<AnswerResponse> Answers { get; set; } = default!;
    public ICollection<CommentResponse> Comments { get; set; } = default!;
}

