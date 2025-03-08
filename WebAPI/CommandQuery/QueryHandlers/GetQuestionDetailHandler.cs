using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionDetailHandler(IQuestionRepository questionRepository,
                                      IAnswerRepository answerRepository,
                                      IBookmarkRepository bookmarkRepository,
                                      QuestionSearchService questionSearchService,
                                      AuthenticationContext authenticationContext)
    : IQueryHandler<GetQuestionDetailQuery, GenericResult<GetQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly IBookmarkRepository _bookmarkRepository = bookmarkRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GetQuestionResponse>> Handle(
        GetQuestionDetailQuery request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindQuestionDetailByIdAsync(request.Id, cancellationToken);

        if (question == null)
        {
            var errMsg = string.Format(EM.QUESTION_ID_NOTFOUND, request.Id);
            return GenericResult<GetQuestionResponse>.Failure(errMsg);
        }

        question.WithAnswerCount(_answerRepository.CountQuestionAnswer(question.Id));

        _questionRepository.MarkAsView(question.Id);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        await questionSearchService.IndexOrUpdateAsync(question, cancellationToken);

        var isQuestionBookmarked =
            await _bookmarkRepository.IsBookmarked(_authenticationContext.UserId, question.Id);

        return GenericResult<GetQuestionResponse>.Success(
            question
            .ToGetQuestionResponse(_authenticationContext.UserId)
            .SetResourceRight(_authenticationContext.UserId)
            .SetIsBookmared(isQuestionBookmarked));
    }
}
