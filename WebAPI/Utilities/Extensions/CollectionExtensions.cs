using Riok.Mapperly.Abstractions;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Response.QuestionResponses;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class CollectionExtensions
{
    public static partial Collection ToCollection(this CreateCollectionDto source);
    public static partial GetCollectionResponse ToGetCollectionResponse(this Collection source);
    public static partial GetCollectionDetailResponse ToCollectionDetailResponse(this Collection obj, bool isLikedByUser,
        PagedResponse<GetQuestionResponse> questions);
}