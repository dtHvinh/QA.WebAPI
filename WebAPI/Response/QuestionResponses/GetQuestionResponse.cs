using WebAPI.Response.AppUserResponses;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Response.CommentResponses;
using WebAPI.Response.HistoryResponses;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.QuestionResponses;

public class GetQuestionResponse : IResourceRight<GetQuestionResponse>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = default!;
    public DateTime UpdatedAt { get; set; } = default!;

    public AuthorResponse? Author { get; set; } = default!;

    public bool IsDuplicate { get; set; } = false;
    public bool IsClosed { get; set; } = false;
    public bool IsSolved { get; set; } = false;
    public bool IsBookmarked { get; set; } = false;

    public int ViewCount { get; set; }
    public int AnswerCount { get; set; }
    public int CommentCount { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }

    public string ResourceRight { get; set; } = nameof(ResourceRights.Viewer);

    public ICollection<TagResponse> Tags { get; set; } = default!;
    public ICollection<AnswerResponse> Answers { get; set; } = default!;
    public ICollection<CommentResponse> Comments { get; set; } = default!;
    public ICollection<QuestionHistoryResponse> Histories { get; set; } = default!;

    public GetQuestionResponse SetResourceRight(int? requesterId)
    {
        if (Author is null || requesterId is null)
            ResourceRight = nameof(ResourceRights.Viewer);
        else
        {
            ResourceRight = Author.Id == requesterId ? nameof(ResourceRights.Owner)
                : nameof(ResourceRights.Viewer);
        }

        return this;
    }

    public GetQuestionResponse SetIsBookmared(bool value)
    {
        IsBookmarked = value;
        return this;
    }
}

