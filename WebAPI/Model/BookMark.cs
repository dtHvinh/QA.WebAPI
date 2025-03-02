using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AuthorId), nameof(CreatedAt), IsDescending = [false, true])]
public class BookMark : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(Author))] public int AuthorId { get; set; }
    public AppUser? Author { get; set; } = null!;

    [ForeignKey(nameof(Question))] public int QuestionId { get; set; }
    public Question? Question { get; set; } = null!;
}