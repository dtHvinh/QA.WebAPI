using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetSingleQuestionHandler(IQuestionRepository questionRepository,
                                      ICacheService cacheService,
                                      AuthenticationContext authenticationContext)
    : IQueryHandler<GetSingleQuestionQuery, GenericResult<GetQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly ICacheService _cacheService = cacheService;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GetQuestionResponse>> Handle(
        GetSingleQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _cacheService.GetQuestionAsync(request.Id);
        question ??= await _questionRepository.FindAvailableQuestionByIdAsync(request.Id, cancellationToken);

        if (question == null)
        {
            var errMsg = string.Format(EM.QUESTION_ID_NOTFOUND, request.Id);
            return GenericResult<GetQuestionResponse>.Failure(errMsg);
        }

        await _cacheService.SetQuestionAsync(question);

        _questionRepository.MarkAsView(question.Id);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return GenericResult<GetQuestionResponse>.Success(
            question.ToGetQuestionResponse().SetResourceRight(_authenticationContext.UserId));
    }
}
