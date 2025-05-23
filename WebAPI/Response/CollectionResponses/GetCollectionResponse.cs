using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Contract;

namespace WebAPI.Response.CollectionResponses;

public record GetCollectionResponse(
    int Id,
    string Name,
    string? Description,
    int LikeCount,
    int QuestionCount,
    bool IsPublic,
    DateTimeOffset CreationDate,
    AuthorResponse Author) : IResourceRight<GetCollectionResponse>
{
    public string ResourceRight { get; set; } = nameof(ResourceRights.Viewer);

    public GetCollectionResponse SetResourceRight(int? requesterId)
    {
        if (requesterId != null && requesterId == Author.Id)
        {
            ResourceRight = nameof(ResourceRights.Owner);
        }

        return this;
    }
}