using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagDetailHandler(ITagRepository tagRepository) : IQueryHandler<GetTagDetailQuery, GenericResult<TagWithWikiBodyResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<TagWithWikiBodyResponse>> Handle(
        GetTagDetailQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.FindTagWithBodyById(request.Id, cancellationToken);

        if (tag is null)
        {
            return GenericResult<TagWithWikiBodyResponse>.Failure(EM.TAG_NOTFOUND);
        }

        return GenericResult<TagWithWikiBodyResponse>.Success(tag.ToTagWithBodyResonse());
    }
}
