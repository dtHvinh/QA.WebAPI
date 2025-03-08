using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetSimilarQuestionHandler(QuestionSearchService questionSearchService)
    : IQueryHandler<GetSimilarQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly QuestionSearchService _questionSearchService = questionSearchService;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(GetSimilarQuestionQuery request, CancellationToken cancellationToken)
    {
        var similarQuestion = await _questionSearchService.SearchSimilarQuestionAsync(
            request.QuestionId, request.Skip, request.Take + 1, cancellationToken);

        var hasNext = similarQuestion.Documents.Count == request.Take + 1;

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new(similarQuestion.Documents.Take(request.Take).Select(e => e.ToGetQuestionResponse()),
            hasNext,
            -1,
            -1));
    }
}
