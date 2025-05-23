using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Response.QuestionResponses;

namespace WebAPI.Utilities.Extensions;

public static class CollectionExtensions
{
    public static Collection ToCollection(this CreateCollectionDto dto)
    {
        return new Collection
        {
            Name = dto.Name,
            Description = dto.Description,
            IsPublic = dto.IsPublic,
        };
    }

    public static GetCollectionResponse ToGetCollectionResponse(this Collection obj)
    {
        return new(obj.Id,
            obj.Name,
            obj.Description,
            obj.LikeCount,
            obj.QuestionCount,
            obj.IsPublic,
            obj.CreationDate,
            obj.Author.ToAuthorResponse()!);
    }

    public static GetCollectionDetailResponse ToCollectionDetailResponse(this Collection obj, bool isLikedByUser,
        PagedResponse<GetQuestionResponse> questions)
    {
        return new(obj.Id,
            obj.Name,
            obj.Description,
            obj.LikeCount,
            obj.IsPublic,
            isLikedByUser,
            obj.CreationDate,
            obj.Author.ToAuthorResponse()!,
            questions);
    }
}