using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagDescriptionHandler(ITagRepository tagRepository)
    : IQueryHandler<GetTagDescriptionQuery, GenericResult<TextResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<TextResponse>> Handle(GetTagDescriptionQuery request, CancellationToken cancellationToken)
    {
        // TODO: Cache the result for an hour (maybe) :)))).
        var tagDescription = await _tagRepository.FindTagDescription(request.TagId, cancellationToken);
        return tagDescription is null
            ? GenericResult<TextResponse>.Failure("")
            : GenericResult<TextResponse>.Success(tagDescription.Content);
    }
}
