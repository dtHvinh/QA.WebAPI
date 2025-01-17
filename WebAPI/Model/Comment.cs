using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Comment : IEntityWithTime<Guid>, ISoftDeleteEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CommentType { get; set; }

    [Column(TypeName = "nvarchar(2000)")]
    public required string Content { get; set; }
    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }
    public AppUser? Author { get; set; } = default!;
}
