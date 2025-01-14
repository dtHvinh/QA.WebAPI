using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Utilities.Response;

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
    public static AuthResponse ToLoginResponseDto(this AppUser user, string accessToken, string refreshToken)
    {
        return new AuthResponse(accessToken, refreshToken, user.UserName!, user.ProfilePicture);
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

    public static Question ToQuestion(this UpdateQuestionDto dto, Guid authorId)
    {
        return new Question
        {
            Id = dto.Id,
            Title = dto.Title,
            Content = dto.Content,
            Slug = dto.Title.GenerateSlug(),
            AuthorId = authorId,
        };
    }
}

public static class TagMap
{
    public static Tag ToTag(this CreateTagDto dto)
    {
        return new Tag
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static Tag ToTag(this UpdateTagDto dto)
    {
        return new Tag
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static DeleteTagResponse ToDeleteTagResponse(this Tag obj)
    {
        return new DeleteTagResponse(obj.Id);
    }

    public static CreateTagResponse ToCreateTagResponse(this Tag obj)
    {
        return new CreateTagResponse(obj.Id, obj.Name, obj.Description);
    }
}
