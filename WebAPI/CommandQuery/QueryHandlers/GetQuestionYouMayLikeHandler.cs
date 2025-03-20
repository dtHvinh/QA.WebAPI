using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionYouMayLikeHandler(IQuestionRepository questionRepository)
    : IQueryHandler<GetQuestionYouMayLikeQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(GetQuestionYouMayLikeQuery request, CancellationToken cancellationToken)
    {
        var result = await _questionRepository.SearchQuestionYouMayLikeAsync(
            (request.PageArgs.PageIndex - 1) * request.PageArgs.PageSize,
            request.PageArgs.PageSize + 1, cancellationToken);

        var hasNext = result.Documents.Count == request.PageArgs.PageSize + 1;

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new(
                result.Documents
                    .Take(request.PageArgs.PageSize).Select(e => e.ToGetQuestionResponse()).ToList(),
                hasNext,
                request.PageArgs.PageIndex,
                request.PageArgs.PageSize));
    }
}
