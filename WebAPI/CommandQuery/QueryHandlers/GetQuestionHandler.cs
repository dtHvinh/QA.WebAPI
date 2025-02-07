using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionHandler(IQuestionRepository questionRepository) : IQueryHandler<GetQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindQuestion(
            (request.PageArgs.Page - 1) * request.PageArgs.PageSize,
            request.PageArgs.PageSize + 1,
            Enum.Parse<QuestionSortOrder>(request.OrderBy, true)
            , cancellationToken);


        var hasNext = question.Count == request.PageArgs.PageSize;
        if (hasNext)
        {
            question.RemoveAt(question.Count - 1);
        }

        var totalCount = await _questionRepository.CountAsync();

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new PagedResponse<GetQuestionResponse>(
                question.Select(q => q.ToGetQuestionResponse()).Take(request.PageArgs.PageSize).ToList(),
                hasNext,
                request.PageArgs.Page,
                request.PageArgs.PageSize)
            {
                TotalCount = totalCount,
                TotalPage = NumericCalcHelper.GetTotalPage(totalCount, request.PageArgs.PageSize)
            });
    }
}
