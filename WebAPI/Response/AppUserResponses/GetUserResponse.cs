using WebAPI.Model;

namespace WebAPI.Response.AppUserResponses;

public class GetUserResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string? Email { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string ProfilePicture { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public int Reputation { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsBanned { get; set; }

    public GetUserResponse SetIsBanned(bool isBanned)
    {
        IsBanned = isBanned;
        return this;
    }
}

public static class GetUserResponseExtensions
{
    public static GetUserResponse ToGetUserResponse(this AppUser user)
    {
        return new GetUserResponse
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePicture = user.ProfilePicture,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted,
            Reputation = user.Reputation
        };
    }
}
