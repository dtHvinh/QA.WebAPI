using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagQuestionHandler(IQuestionRepository questionRepository) : IQueryHandler<GetTagQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(GetTagQuestionQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.FindQuestionsByTagId(
                        request.TagId,
                        Enum.Parse<QuestionSortOrder>(request.Order),
                        (request.PageArgs.PageIndex - 1) * request.PageArgs.PageSize,
                        request.PageArgs.PageSize + 1,
                        cancellationToken);

        var hasNext = questions.Count == request.PageArgs.PageSize + 1;

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new PagedResponse<GetQuestionResponse>(
                questions.Take(request.PageArgs.PageSize).Select(e => e.ToGetQuestionResponse()),
                hasNext,
                request.PageArgs.PageIndex,
                request.PageArgs.PageSize)
        );
    }
}
