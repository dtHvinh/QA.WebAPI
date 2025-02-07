using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateAnswerHandler(IAnswerRepository answerRepository,
                                 IQuestionRepository questionRepository,
                                 AuthenticationContext authContext)
    : ICommandHandler<CreateAnswerCommand, GenericResult<AnswerResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<AnswerResponse>> Handle(
        CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
            return GenericResult<AnswerResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        if (question.IsClosed)
            return GenericResult<AnswerResponse>.Failure(EM.QUESTION_CLOSED_COMMENT_RESTRICT);


        var newAnswer = request.Answer.ToAnswer(_authContext.UserId, request.QuestionId);
        _answerRepository.AddAnswer(newAnswer);

        var result = await _answerRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<AnswerResponse>.Success(newAnswer.ToAnswerResponse().SetResourceRight(_authContext.UserId))
            : GenericResult<AnswerResponse>.Failure(result.Message);
    }
}
