﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AuthorId))]
[Index(nameof(CreatedAt), AllDescending = true)]
[Index(nameof(ViewCount), AllDescending = true)]
[Index(nameof(IsSolved))]
[Index(nameof(Score))]
public class Question : IEntityWithTime<int>, ISoftDeleteEntity, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }


    [ForeignKey(nameof(Author))]
    public required int AuthorId { get; set; }
    public AppUser? Author { get; set; } = default!;

    [Column(TypeName = "nvarchar(150)")]
    public string Title { get; set; } = default!;
    [Column(TypeName = "nvarchar(250)")]
    public string Slug { get; set; } = default!;
    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = default!;

    public string DuplicateQuestionUrl { get; set; } = ""; // When user mark as duplicate
    public bool IsDuplicate { get; set; } = false; // When user or admin mark as duplicate

    public bool IsClosed { get; set; } = false; // Disable new answer
    public bool IsDeleted { get; set; } = false;
    public bool IsSolved { get; set; } = false; // When author accept one of the answers

    public int ViewCount { get; set; }
    public int AnswerCount { get; set; }
    public int CommentCount { get; set; }
    public int Score { get; set; }

    public ICollection<Answer> Answers { get; set; } = default!;
    public ICollection<QuestionVote> Votes { get; set; } = default!;
    public ICollection<Tag> Tags { get; set; } = default!;
    public ICollection<QuestionComment> Comments { get; set; } = default!;
    public ICollection<QuestionHistory> QuestionHistories { get; set; } = default!;
    public ICollection<Collection> Collections { get; set; } = default!;
}

public enum QuestionSortOrder
{
    Newest,
    MostVoted,
    MostViewed,
    Solved,
}