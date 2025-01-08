using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(IsClosed), nameof(IsHide), nameof(IsDeleted))]
public class Question : IEntity<Guid>, ISoftDeleteEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }
    public AppUser Author { get; set; } = default!;

    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Content { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }

    public bool IsDuplicate { get; set; } // When user or admin mark as duplicate
    public bool IsClosed { get; set; } // Disable new answer
    public bool IsHide { get; set; } // When user save current state of the Question
    public bool IsDeleted { get; set; }

    public int ViewCount { get; set; }
    public int AnswerCount { get; set; }

    public ICollection<Answer> Answers { get; set; } = default!;
    public ICollection<Upvote> Upvotes { get; set; } = default!;
    public ICollection<Downvote> Downvotes { get; set; } = default!;
    public ICollection<QuestionReport> Reports { get; set; } = default!;
    public ICollection<QuestionComment> Comments { get; set; } = default!;
    public ICollection<QuestionTag> QuestionTags { get; set; } = default!;
}
