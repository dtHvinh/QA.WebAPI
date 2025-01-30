using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagDetailHandler(ITagRepository tagRepository)
    : IQueryHandler<GetTagDetailQuery, GenericResult<TagDetailResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<TagDetailResponse>> Handle(GetTagDetailQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.FindTagDetailById(request.TagId, cancellationToken);
        if (tag == null)
        {
            return GenericResult<TagDetailResponse>.Failure("Tag not found");
        }


        return GenericResult<TagDetailResponse>.Success(tag.ToTagDetailResponse());
    }
}
