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

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; }
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
    public static GetUserResponse ToGetUserResponse(this ApplicationUser user)
    {
        return new GetUserResponse
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            ProfilePicture = user.ProfilePicture,
            CreatedAt = user.CreationDate,
            UpdatedAt = user.ModificationDate,
            IsDeleted = user.IsDeleted,
            Reputation = user.Reputation
        };
    }
}
