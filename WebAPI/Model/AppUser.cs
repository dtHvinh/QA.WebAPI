using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(IsDeleted), nameof(IsBanned))]
public class AppUser : IdentityUser<Guid>, ISoftDeleteEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int Reputation { get; set; }
    public required bool IsBanned { get; set; }
    public required DateTime DateJoined { get; set; } = DateTime.UtcNow;
    public required DateTime LastActive { get; set; }
    public required string ProfilePicture { get; set; }
    public required string Bio { get; set; }
    public bool IsDeleted { get; set; } = false;

    public ICollection<Question> Questions { get; set; } = default!;
    public ICollection<Answer> Answers { get; set; } = default!;
    public ICollection<Downvote> Downvotes { get; set; } = default!;
    public ICollection<Upvote> Upvotes { get; set; } = default!;
}
