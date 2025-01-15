using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Utilities.Response.AppUserResponses;
using WebAPI.Utilities.Response.AuthResponses;

namespace WebAPI.Utilities.Mappers;

public static class AppUserMap
{
    public static AuthResponse ToLoginResponseDto(this AppUser user, string accessToken, string refreshToken)
    {
        return new AuthResponse(accessToken, refreshToken, user.UserName!, user.ProfilePicture);
    }

    public static AppUser ToAppUser(this RegisterDto dto)
    {
        return new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };
    }

    public static AuthorResponse? ToAuthorResponse(this AppUser? obj)
    {
        return obj is null ? null :
            new AuthorResponse
            {
                Id = obj.Id,
                Username = obj.UserName,
                ProfilePicture = obj.ProfilePicture,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Reputation = obj.Reputation,
            };
    }

}
