using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetSingleAvailableQuestionHandler(IQuestionRepository questionRepository,
                                               ICacheService cacheService)
    : IQueryHandler<GetSingleAvailableQuestionQuery, OperationResult<GetQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<OperationResult<GetQuestionResponse>> Handle(
        GetSingleAvailableQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _cacheService.GetQuestionAsync(request.Id);
        if (question == null)
            question = await _questionRepository.FindAvailableQuestionByIdAsync(request.Id, cancellationToken);

        if (question == null)
        {
            var errMsg = string.Format(EM.QUESTION_ID_NOTFOUND, request.Id);
            return OperationResult<GetQuestionResponse>.Failure(errMsg);
        }

        await _cacheService.SetQuestionAsync(question);

        _questionRepository.MarkAsView(question.Id);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<GetQuestionResponse>.Success(question.ToGetQuestionResponse());
    }
}
