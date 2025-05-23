using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(NormalizedName), IsUnique = true)]
[Index(nameof(QuestionCount), IsDescending = [true])]
public class Tag : IEntityWithTime<int>
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public string NormalizedName { get; set; } = default!;
    public int QuestionCount { get; set; }

    public TagDescription? Description { get; set; }
    public TagBody? WikiBody { get; set; }

    [JsonIgnore]
    public ICollection<Question> Questions { get; set; } = default!;

    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }
}
