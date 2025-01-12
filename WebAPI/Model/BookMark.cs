using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(UserId), nameof(CreatedAt), IsDescending = [false, true])]
public class BookMark : IEntityWithTime<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;

    [ForeignKey(nameof(Question))]
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; } = default!;
}
