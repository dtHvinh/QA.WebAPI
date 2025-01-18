using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Params;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class SearchQuestionHandler(IQuestionRepository questionRepository)
    : IQueryHandler<SearchQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(
        SearchQuestionQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Args.Page - 1) * request.Args.PageSize;
        var searchParams = QuestionSearchParams.From(
            request.Keyword, request.TagId, skip, request.Args.PageSize + 1);

        var questions = await _questionRepository.SearchQuestionAsync(searchParams, cancellationToken);

        var hasNext = questions.Count > request.Args.PageSize;

        if (hasNext)
            questions.RemoveAt(questions.Count - 1);

        var response = new PagedResponse<GetQuestionResponse>(
            questions.Select(e => e.ToGetQuestionResponse()), hasNext, request.Args.Page, request.Args.PageSize);

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(response);
    }
}