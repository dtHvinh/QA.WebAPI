using WebAPI.Dto;
using WebAPI.Model;

namespace WebAPI.Utilities.Mappers;

public static class MapExtensions
{
    public static AppUser ToAppUser(this RegisterDto dto)
    {
        return new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };
    }

    public static string GenerateSlug(this string title)
    {
        return title.Trim().ToLower().Replace(" ", "-");
    }
}

public static class AppUserMap
{
    public static AuthResponseDto ToLoginResponseDto(this AppUser user, string accessToken, string refreshToken)
    {
        return new AuthResponseDto(accessToken, refreshToken, user.UserName!, user.ProfilePicture);
    }
}

public static class QuestionMap
{
    public static Question ToQuestion(this CreateQuestionDto dto, Guid authorId)
    {
        return new Question
        {
            Title = dto.Title,
            Content = dto.Content,
            Slug = dto.Title.GenerateSlug(),
            AuthorId = authorId
        };
    }
}
