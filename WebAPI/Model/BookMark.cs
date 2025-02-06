using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AuthorId), nameof(CreatedAt), IsDescending = [false, true])]
public class BookMark : IEntityWithTime<Guid>, IOwnedByUser<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }
    public AppUser? Author { get; set; } = default!;

    [ForeignKey(nameof(Question))]
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; } = default!;
}
