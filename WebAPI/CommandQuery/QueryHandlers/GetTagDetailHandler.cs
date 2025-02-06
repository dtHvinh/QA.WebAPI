using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagDetailHandler(ITagRepository tagRepository) : IQueryHandler<GetTagDetailQuery, GenericResult<TagResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<TagResponse>> Handle(GetTagDetailQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.FindTagById(request.Id, cancellationToken);

        if (tag is null)
        {
            return GenericResult<TagResponse>.Failure(EM.TAG_NOTFOUND);
        }

        return GenericResult<TagResponse>.Success(tag.ToTagResonse());
    }
}
