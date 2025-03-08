using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Response.TagResponses;

namespace WebAPI.Utilities.Extensions;

public static class TagExtensions
{
    public static Tag ToTag(this CreateTagDto dto)
    {
        return new Tag
        {
            Name = dto.Name,
        };
    }

    public static Tag ToTag(this UpdateTagDto dto)
    {
        return new Tag
        {
            Id = dto.Id,
            Name = dto.Name,
        };
    }

    public static TagWithWikiBodyResponse ToTagWithBodyResonse(this Tag? obj)
    {
        if (obj == null)
        {
            return null;
        }
        return new TagWithWikiBodyResponse(obj.Id, obj.Name, obj.Description!.Content, obj.WikiBody!.Content, obj.QuestionCount);
    }

    public static TagResponse? ToTagResonse(this Tag? obj)
    {
        if (obj == null)
        {
            return null;
        }
        return new TagResponse(obj.Id, obj.Name, obj.Description?.Content, obj.QuestionCount);
    }

    public static TagWithQuestionResponse ToTagWithQuestionResponse(this Tag obj, PagedResponse<GetQuestionResponse> questions)
    {
        return new TagWithQuestionResponse(
            obj.Id, obj.Name, obj.Description!.Content,
            obj.QuestionCount, questions);
    }
}
