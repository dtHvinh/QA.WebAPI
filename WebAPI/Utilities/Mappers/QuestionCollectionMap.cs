using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Response.QuestionResponses;

namespace WebAPI.Utilities.Mappers;

public static class QuestionCollectionMap
{
    public static QuestionCollection ToQuestionCollection(this CreateCollectionDto dto)
    {
        return new QuestionCollection
        {
            Name = dto.Name,
            Description = dto.Description,
            IsPublic = dto.IsPublic,
        };
    }

    public static GetCollectionResponse ToGetCollectionResponse(this QuestionCollection obj)
    {
        return new(obj.Id,
                   obj.Name,
                   obj.Description,
                   obj.LikeCount,
                   obj.IsPublic,
                   obj.CreatedAt,
                   obj.Author.ToAuthorResponse()!);
    }

    public static GetCollectionDetailResponse ToCollectionDetailResponse(this QuestionCollection obj, PagedResponse<GetQuestionResponse> questions)
    {
        return new(obj.Id,
                   obj.Name,
                   obj.Description,
                   obj.LikeCount,
                   obj.IsPublic,
                   obj.CreatedAt,
                   obj.Author.ToAuthorResponse()!,
                   questions);
    }
}
