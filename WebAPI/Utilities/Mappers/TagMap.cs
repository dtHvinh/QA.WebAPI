using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Response.TagResponses;

namespace WebAPI.Utilities.Mappers;

public static class TagMap
{
    public static Tag ToTag(this CreateTagDto dto)
    {
        return new Tag
        {
            Name = dto.Name,
            Description = dto.Description,
            WikiBody = dto.WikiBody
        };
    }

    public static Tag ToTag(this UpdateTagDto dto)
    {
        return new Tag
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            WikiBody = dto.WikiBody
        };
    }

    public static TagResponse ToTagResonse(this Tag obj)
    {
        if (obj == null)
        {
            return null;
        }
        return new TagResponse(obj.Id, obj.Name, obj.Description, obj.WikiBody, obj.QuestionCount);
    }

    public static TagWithQuestionResponse ToTagWithQuestionResponse(this Tag obj, PagedResponse<GetQuestionResponse> questions)
    {
        return new TagWithQuestionResponse(
            obj.Id, obj.Name, obj.Description, obj.WikiBody,
            obj.QuestionCount, questions);
    }

    public static DeleteTagResponse ToDeleteTagResponse(this Tag obj)
    {
        return new DeleteTagResponse(obj.Id);
    }

    public static CreateTagResponse ToCreateTagResponse(this Tag obj)
    {
        return new CreateTagResponse(obj.Id, obj.Name, obj.Description, obj.WikiBody);
    }
}
