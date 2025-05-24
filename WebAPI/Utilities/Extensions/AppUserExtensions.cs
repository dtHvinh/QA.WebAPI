using Riok.Mapperly.Abstractions;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.AppUserResponses;
using WebAPI.Response.AuthResponses;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class AppUserExtensions
{
    public static partial AuthResponse ToAuthResponseDto(this ApplicationUser source,
        string accessToken, string refreshToken, IList<string> roles);

    public static ApplicationUser ToAppUser(this RegisterDto dto)
    {
        return new ApplicationUser
        {
            UserName = $"user{Random.Shared.Next(100)}{Random.Shared.Next(100000)}",
            Email = dto.Email
        };
    }

    public static partial AuthorResponse ToAuthorResponse(this ApplicationUser source);
    public static partial UserResponse ToUserResponse(this ApplicationUser source);

}