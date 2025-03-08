using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionHandler(IQuestionRepository questionRepository) : IQueryHandler<GetQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.FindQuestion(
            (request.PageArgs.PageIndex - 1) * request.PageArgs.PageSize,
            request.PageArgs.PageSize + 1,
            Enum.Parse<QuestionSortOrder>(request.OrderBy, true)
            , cancellationToken);


        var hasNext = questions.Count == request.PageArgs.PageSize;
        if (hasNext)
        {
            questions.RemoveAt(questions.Count - 1);
        }

        var totalCount = await _questionRepository.CountAsync();

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new PagedResponse<GetQuestionResponse>(
                questions.Select(q => q.ToGetQuestionResponse()).Take(request.PageArgs.PageSize).ToList(),
                hasNext,
                request.PageArgs.PageIndex,
                request.PageArgs.PageSize)
            {
                TotalCount = totalCount,
                TotalPage = MathHelper.GetTotalPage(totalCount, request.PageArgs.PageSize)
            });
    }
}
