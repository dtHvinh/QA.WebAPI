using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetAllTagHandler(ITagRepository tagRepository) : IQueryHandler<GetAllTagQuery, GenericResult<List<TagResponse>>>
{
    private readonly ITagRepository tagRepository = tagRepository;

    public async Task<GenericResult<List<TagResponse>>> Handle(GetAllTagQuery request, CancellationToken cancellationToken)
    {
        var tags = await tagRepository.FindAllAsync(cancellationToken);
        var tagResponses = tags.Select(tag => tag.ToTagResonse()).ToList();
        return GenericResult<List<TagResponse>>.Success(tagResponses);
    }
}
