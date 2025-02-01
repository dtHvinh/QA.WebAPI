using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagDetailHandler(ITagRepository tagRepository, ICacheService cacheService)
    : IQueryHandler<GetTagDetailQuery, GenericResult<TagDetailResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ICacheService cacheService = cacheService;

    public async Task<GenericResult<TagDetailResponse>> Handle(GetTagDetailQuery request, CancellationToken cancellationToken)
    {
        Tag? tag = await cacheService.GetTagDetail(request.TagId);

        if (tag == null)
        {
            tag = await _tagRepository.FindTagDetailById(request.TagId, cancellationToken);

            if (tag == null)
                return GenericResult<TagDetailResponse>.Failure(EM.TAG_ID_NOTFOUND);

            await cacheService.SetTagDetail(tag);
        }


        return GenericResult<TagDetailResponse>.Success(tag.ToTagDetailResponse());
    }
}
