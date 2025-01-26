using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(IsDraft), nameof(IsClosed), nameof(IsDeleted))]
public class Question : IEntityWithTime<Guid>, ISoftDeleteEntity, IOwnedByUser<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }


    [ForeignKey(nameof(Author))]
    public required Guid AuthorId { get; set; }
    public AppUser? Author { get; set; } = default!;

    [Column(TypeName = "nvarchar(150)")]
    public required string Title { get; set; }
    [Column(TypeName = "nvarchar(250)")]
    public required string Slug { get; set; }
    [Column(TypeName = "nvarchar(max)")]
    public required string Content { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }

    public bool IsDuplicate { get; set; } = false; // When user or admin mark as duplicate
    public bool IsClosed { get; set; } = false; // Disable new answer
    public bool IsDraft { get; set; } = false;// When user save current state of the Question
    public bool IsDeleted { get; set; } = false;
    public bool IsSolved { get; set; } = false; // When author accept one of the answers

    public int ViewCount { get; set; }
    public int AnswerCount { get; set; }
    public int CommentCount { get; set; }

    public ICollection<Answer> Answers { get; set; } = default!;
    public ICollection<QuestionVote> Votes { get; set; } = default!;
    public ICollection<QuestionReport> Reports { get; set; } = default!;
    public ICollection<QuestionComment> Comments { get; set; } = default!;
    public ICollection<Tag> Tags { get; set; } = default!;
}

public enum QuestionSortOrder
{
    Newest,
    MostVoted,
    MostViewed,
    Solved
}