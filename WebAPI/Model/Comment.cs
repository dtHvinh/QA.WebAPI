using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Comment : IEntityWithTime<int>, ISoftDeleteEntity, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }
    public string? CommentType { get; set; }
    public bool IsDeleted { get; set; }

    public string Content { get; set; } = default!;

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; } = default!;
}

public enum CommentTypes
{
    Question,
    Answer
}