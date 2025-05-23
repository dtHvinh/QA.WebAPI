using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(IsDeleted))]
[Index(nameof(Reputation))]
public class ApplicationUser : IdentityUser<int>, IEntityWithTime<int>, ISoftDeleteEntity
{
    public string? RefreshToken { get; set; }
    public int Reputation { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTime.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }

    public string? ProfilePicture { get; set; } = default!;
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public ICollection<Question> Questions { get; set; } = default!;
    [JsonIgnore]
    public ICollection<Answer> Answers { get; set; } = default!;
    [JsonIgnore]
    public ICollection<Vote> Votes { get; set; } = default!;
    [JsonIgnore]
    public ICollection<BookMark> BookMarks { get; set; } = default!;
    [JsonIgnore]
    public ICollection<ExternalLinks> ExternalLinks { get; set; } = default!;
}
