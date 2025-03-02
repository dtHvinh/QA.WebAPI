using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;

namespace WebAPI.CommandQuery.QueryHandlers;

public class SearchQuestionHandler(QuestionSearchService questionSearchService)
    : IQueryHandler<SearchQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly QuestionSearchService _questionSearchService = questionSearchService;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(
        SearchQuestionQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Args.Page - 1) * request.Args.PageSize;

        var questions =
            request.TagId == 0
                ? await _questionSearchService.SearchQuestionNoTagAsync(request.Keyword, skip,
                    request.Args.PageSize + 1, cancellationToken)
                : await _questionSearchService.SearchQuestionAsync(
                    request.Keyword, request.TagId, skip, request.Args.PageSize + 1,
                    cancellationToken);

        var hasNext = questions.Documents.Count == request.Args.PageSize + 1;

        var response = new PagedResponse<GetQuestionResponse>(
            questions
                .Documents
                .Take(request.Args.PageSize)
                .Select(e => e.ToGetQuestionResponse()),
            hasNext,
            request.Args.Page,
            request.Args.PageSize)
        {
            TotalCount = (int)questions.Total,
            TotalPage = NumericCalcHelper.GetTotalPage((int)questions.Total, request.Args.PageSize)
        };

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(response);
    }
}