﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(QuestionId))]
public class Answer : IEntityWithTime<int>, IOwnedByUser<int>, ISoftDeleteEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question? Question { get; set; } = default!;

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public AppUser? Author { get; set; } = default!;
    [Column(TypeName = "nvarchar(max)")]
    public required string Content { get; set; }
    public int Score { get; set; }

    public bool IsDeleted { get; set; } = false;
    public bool IsAccepted { get; set; } = false;

    public ICollection<AnswerVote> Votes { get; set; } = default!;
    public ICollection<AnswerComment> Comments { get; set; } = default!;
}
