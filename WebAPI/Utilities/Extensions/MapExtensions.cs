using WebAPI.Dto;
using WebAPI.Model;

namespace WebAPI.Utilities.Extensions;

public static class MapExtensions
{
    public static AppUser ToAppUser(this RegisterDto dto)
    {
        return new AppUser
        {
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
    }

    public static AuthResponseDto ToLoginResponseDto(this AppUser user, string accessToken, string refreshToken)
    {
        return new AuthResponseDto(accessToken, refreshToken, user.UserName!, user.ProfilePicture);
    }
}
