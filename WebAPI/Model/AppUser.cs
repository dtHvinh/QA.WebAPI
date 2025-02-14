using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(IsDeleted), nameof(IsBanned))]
public class AppUser : IdentityUser<int>, ISoftDeleteEntity
{
    [Column(TypeName = "nvarchar(50)")]
    public string? FirstName { get; set; }
    [Column(TypeName = "nvarchar(50)")]
    public string? LastName { get; set; }
    public int Reputation { get; set; }
    public bool IsBanned { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; }
    [Column(TypeName = "nvarchar(255)")]
    public string ProfilePicture { get; set; } = default!;
    [Column(TypeName = "nvarchar(255)")]
    public string Bio { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public ICollection<Question> Questions { get; set; } = default!;
    [JsonIgnore]
    public ICollection<Answer> Answers { get; set; } = default!;
    [JsonIgnore]
    public ICollection<Vote> Votes { get; set; } = default!;
    [JsonIgnore]
    public ICollection<BookMark> BookMarks { get; set; } = default!;
}
