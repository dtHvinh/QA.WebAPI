using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class ModGetQuestionHandler(
    IQuestionRepository questionRepository,
    AuthenticationContext authenticationContext)
    : IQueryHandler<ModGetQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(ModGetQuestionQuery request, CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsModerator())
            return GenericResult<PagedResponse<GetQuestionResponse>>.Failure("You are not authorized to perform this action.");

        var questions = await _questionRepository.FindQuestionNoException(
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageSize + 1,
            cancellationToken);

        var hasNext = questions.Count > request.PageArgs.PageSize;

        var total = await _questionRepository.CountAsync();

        var response = new PagedResponse<GetQuestionResponse>(
            questions.Take(request.PageArgs.PageSize).Select(q => q.ToGetQuestionResponse()),
            hasNext,
            request.PageArgs.PageIndex,
            request.PageArgs.PageSize)
        {
            TotalCount = total,
            TotalPage = MathHelper.GetTotalPage(total, request.PageArgs.PageSize)
        };


        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(response);
    }
}
