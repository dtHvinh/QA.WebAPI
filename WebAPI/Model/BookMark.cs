using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(AuthorId), nameof(CreationDate), IsDescending = [false, true])]
public class BookMark : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }

    [ForeignKey(nameof(Author))] public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; } = null!;

    [ForeignKey(nameof(Question))] public int QuestionId { get; set; }
    public Question? Question { get; set; } = null!;
}