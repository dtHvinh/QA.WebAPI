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
            (request.Args.Page - 1) * request.Args.PageSize,
            request.Args.PageSize + 1,
            request.Order switch
            {
                "Newest" => QuestionSortOrder.Newest,
                "MostViewed" => QuestionSortOrder.MostViewed,
                "MostVoted" => QuestionSortOrder.MostVoted,
                "Solved" => QuestionSortOrder.Solved,
                "Draft" => QuestionSortOrder.Draft,
                _ => QuestionSortOrder.Newest
            }, cancellationToken);

        var hasNext = question.Count > request.Args.PageSize;

        if (hasNext)
        {
            question.RemoveAt(question.Count - 1);
        }

        int count = await _questionRepository.CountUserQuestion(_authenticationContext.UserId);

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new PagedResponse<GetQuestionResponse>(
                question.Select(q => q.ToGetQuestionResponse()).ToList(),
                hasNext,
                request.Args.Page,
                request.Args.PageSize)
            {
                TotalCount = count,
                TotalPage = NumericCalcHelper.GetTotalPage(count, request.Args.PageSize)
            });
    }
}
