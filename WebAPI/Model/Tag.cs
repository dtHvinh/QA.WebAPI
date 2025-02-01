using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(NormalizedName), IsUnique = true)]
[Index(nameof(QuestionCount), IsDescending = [true])]
public class Tag : IEntity<Guid>
{
    public Guid Id { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public required string Name { get; set; }
    [Column(TypeName = "nvarchar(50)")]
    public string NormalizedName { get; set; } = default!;
    [Column(TypeName = "nvarchar(1000)")]
    public string Description { get; set; } = default!;
    public string WikiBody { get; set; } = default!;
    public int QuestionCount { get; set; }

    public ICollection<Question> Questions { get; set; } = default!;
}
