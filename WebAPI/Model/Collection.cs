using Microsoft.EntityFrameworkCore;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AuthorId))]
[Index(nameof(LikeCount))]
[Index(nameof(CreationDate), AllDescending = true)]
[Index(nameof(IsPublic))]
public class Collection : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }

    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    public string Name { get; set; } = null!;
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