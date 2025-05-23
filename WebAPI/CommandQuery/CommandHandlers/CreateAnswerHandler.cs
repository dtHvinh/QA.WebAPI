using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateAnswerHandler(IAnswerRepository answerRepository,
                                 IQuestionRepository questionRepository,
                                 AuthenticationContext authContext,
                                 IQuestionHistoryRepository questionHistoryRepository,
                                 Serilog.ILogger logger)
    : ICommandHandler<CreateAnswerCommand, GenericResult<AnswerResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly IQuestionHistoryRepository _questionHistoryRepository = questionHistoryRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<AnswerResponse>> Handle(
        CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
            return GenericResult<AnswerResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        if (question.IsClosed)
            return GenericResult<AnswerResponse>.Failure(EM.QUESTION_CLOSED_COMMENT_RESTRICT);

        var newAnswer = request.Answer.ToAnswer(_authContext.UserId, request.QuestionId);

        await _answerRepository.AddAnswerAndLoadAuthor(newAnswer, cancellationToken);

        await _questionHistoryRepository.AddHistory(
             question.Id, _authContext.UserId, QuestionHistoryTypes.AddAnswer, request.Answer.Content);

        var result = await _answerRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authContext.UserId, LogOp.Created, newAnswer);

        return result.IsSuccess
            ? GenericResult<AnswerResponse>.Success(newAnswer.ToAnswerResponse().SetResourceRight(_authContext.UserId))
            : GenericResult<AnswerResponse>.Failure(result.Message);
    }
}
