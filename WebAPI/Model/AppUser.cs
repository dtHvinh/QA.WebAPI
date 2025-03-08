using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(IsDeleted))]
[Index(nameof(Reputation))]
public class AppUser : IdentityUser<int>, IEntityWithTime<int>, ISoftDeleteEntity
{
    [Column(TypeName = "nvarchar(50)")]
    public string? FirstName { get; set; }
    [Column(TypeName = "nvarchar(50)")]
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public int Reputation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    [Column(TypeName = "nvarchar(255)")]
    public string ProfilePicture { get; set; } = default!;
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
