using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AuthorId))]
[Index(nameof(LikeCount))]
[Index(nameof(CreatedAt), AllDescending = true)]
[Index(nameof(IsPublic))]
public class Collection : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public int AuthorId { get; set; }
    public AppUser? Author { get; set; }

    [Required][Column("nvarchar(255)")] public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public bool IsPublic { get; set; } = false;

    public int QuestionCount { get; set; }
    public int LikeCount { get; set; }

    public ICollection<Question> Questions { get; set; } = null!;
}

public enum CollectionSortOrder
{
    Newest,
    MostLiked
}