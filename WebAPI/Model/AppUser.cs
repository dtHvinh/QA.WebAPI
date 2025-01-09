using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(IsDeleted), nameof(IsBanned))]
public class AppUser : IdentityUser<Guid>, ISoftDeleteEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int Reputation { get; set; }
    public bool IsBanned { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; }
    public string ProfilePicture { get; set; } = default!;
    public string Bio { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;

    public ICollection<Question> Questions { get; set; } = default!;
    public ICollection<Answer> Answers { get; set; } = default!;
    public ICollection<Downvote> Downvotes { get; set; } = default!;
    public ICollection<Upvote> Upvotes { get; set; } = default!;
    public ICollection<BookMark> BookMarks { get; set; } = default!;
}
