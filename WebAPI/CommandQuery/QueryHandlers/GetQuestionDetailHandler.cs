using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionDetailHandler(IQuestionRepository questionRepository,
                                      ICommentRepository commentRepository,
                                      IAnswerRepository answerRepository,
                                      ICacheService cacheService,
                                      AuthenticationContext authenticationContext)
    : IQueryHandler<GetQuestionDetailQuery, GenericResult<GetQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly ICacheService _cacheService = cacheService;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GetQuestionResponse>> Handle(
        GetQuestionDetailQuery request, CancellationToken cancellationToken)
    {
        var question = await _cacheService.GetQuestionAsync(request.Id);
        question ??= await _questionRepository.FindQuestionDetailByIdAsync(request.Id, cancellationToken);

        if (question == null)
        {
            var errMsg = string.Format(EM.QUESTION_ID_NOTFOUND, request.Id);
            return GenericResult<GetQuestionResponse>.Failure(errMsg);
        }

        question.WithCommentCount(_commentRepository.CountQuestionComment(question.Id))
                .WithAnswerCount(_answerRepository.CountQuestionAnswer(question.Id));

        await _cacheService.SetQuestionAsync(question);

        _questionRepository.MarkAsView(question.Id);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return GenericResult<GetQuestionResponse>.Success(
            question
            .ToGetQuestionResponse(_authenticationContext.UserId)
            .SetResourceRight(_authenticationContext.UserId));
    }
}
