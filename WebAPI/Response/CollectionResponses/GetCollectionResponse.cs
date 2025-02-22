using WebAPI.Response.AppUserResponses;

namespace WebAPI.Response.CollectionResponses;

public record GetCollectionResponse(int Id,
                                    string Name,
                                    string? Description,
                                    int LikeCount,
                                    bool IsPublic,
                                    DateTime CreatedAt,
                                    AuthorResponse Author);
