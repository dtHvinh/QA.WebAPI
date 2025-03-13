using Microsoft.Extensions.Options;
using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class AcceptAnswerHandler(IQuestionRepository questionRepository,
                                 IAnswerRepository answerRepository,
                                 AuthenticationContext authenticationContext,
                                 IQuestionHistoryRepository questionHistoryRepository,
                                 IOptions<ApplicationProperties> options,
                                 Serilog.ILogger logger)
    : ICommandHandler<AcceptAnswerCommand, GenericResult<TextResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly IQuestionHistoryRepository _questionHistoryRepository = questionHistoryRepository;
    private readonly Serilog.ILogger _logger = logger;
    private readonly ApplicationProperties _options = options.Value;

    public async Task<GenericResult<TextResponse>> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
    {
        // Validate things
        var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);
        var answer = await _answerRepository.FindAnswerWithAuthorById(request.AnswerId, cancellationToken);

        if (question is null)
            return GenericResult<TextResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));
        if (answer is null)
            return GenericResult<TextResponse>.Failure(string.Format(EM.ANSWER_ID_NOTFOUND, request.QuestionId));
        if (!await _authenticationContext.IsModerator())
            return GenericResult<TextResponse>.Failure("Need moderator role to accept this question");
        if (answer.IsAccepted)
            return GenericResult<TextResponse>.Failure("Answer is already been accepted");

        // Main logic
        question.IsSolved = true;
        answer.IsAccepted = true;
        answer.Author!.Reputation += _options.ReputationAcquirePerAction.AnswerAccepted;

        _questionHistoryRepository.AddHistory(question.Id, _authenticationContext.UserId, QuestionHistoryTypes.AcceptAnswer, answer.Content);

        await _questionRepository.UpdateQuestion(question);
        _answerRepository.UpdateAnswer(answer);

        var result = await _questionRepository.SaveChangesAsync(cancellationToken);

        _logger.ModeratorAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogModeratorOp.Approved, answer.Author, LogOp.Created, answer);

        return result.IsSuccess
            ? GenericResult<TextResponse>.Success(new("Done"))
            : GenericResult<TextResponse>.Failure(result.Message);

    }
}
