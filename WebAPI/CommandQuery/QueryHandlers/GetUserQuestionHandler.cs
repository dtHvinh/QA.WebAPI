using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetUserQuestionHandler(
    IQuestionRepository questionRepository,
    AuthenticationContext authenticationContext)
    : IQueryHandler<GetUserQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(GetUserQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindQuestionByUserId(
            _authenticationContext.UserId,
            (request.PageArgs.Page - 1) * request.PageArgs.PageSize,
            request.PageArgs.PageSize + 1,
            Enum.Parse<QuestionSortOrder>(request.Order, true),
            cancellationToken);

        var hasNext = question.Count == request.PageArgs.PageSize + 1;

        int count = await _questionRepository.CountUserQuestion(_authenticationContext.UserId, cancellationToken);

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new PagedResponse<GetQuestionResponse>(
                question.Select(q => q.ToGetQuestionResponse()).Take(request.PageArgs.PageSize).ToList(),
                hasNext,
                request.PageArgs.Page,
                request.PageArgs.PageSize)
            {
                TotalCount = count,
                TotalPage = MathHelper.GetTotalPage(count, request.PageArgs.PageSize)
            });
    }
}
