using WebAPI.Pagination;
using WebAPI.Response.AppUserResponses;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.CollectionResponses;

public record GetCollectionDetailResponse(int Id,
                                          string Name,
                                          string? Description,
                                          int LikeCount,
                                          bool IsPublic,
                                          DateTime CreatedAt,
                                          AuthorResponse Author,
                                          PagedResponse<GetQuestionResponse> Questions) : IResourceRight<GetCollectionDetailResponse>
{
    public string ResourceRight { get; set; } = nameof(ResourceRights.Viewer);

    public GetCollectionDetailResponse SetResourceRight(int? requesterId)
    {
        if (Author is null || requesterId is null)
            ResourceRight = nameof(ResourceRights.Viewer);
        else
        {
            ResourceRight = Author.Id == requesterId ? nameof(ResourceRights.Owner)
                : nameof(ResourceRights.Viewer);
        }
        return this;
    }
}
