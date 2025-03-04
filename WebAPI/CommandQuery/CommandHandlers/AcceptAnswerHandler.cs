using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class AcceptAnswerHandler(IQuestionRepository questionRepository,
                                 IAnswerRepository answerRepository,
                                 AuthenticationContext authenticationContext,
                                 IQuestionHistoryRepository questionHistoryRepository,
                                 QuestionSearchService questionSearchService,
                                 IOptions<ApplicationProperties> options,
                                 Serilog.ILogger logger)
    : ICommandHandler<AcceptAnswerCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly IQuestionHistoryRepository _questionHistoryRepository = questionHistoryRepository;
    private readonly QuestionSearchService _questionSearchService = questionSearchService;
    private readonly Serilog.ILogger _logger = logger;
    private readonly ApplicationProperties _options = options.Value;

    public async Task<GenericResult<GenericResponse>> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
    {
        // Validate things
        var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
            return GenericResult<GenericResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        var answer = await _answerRepository.FindAnswerWithAuthorById(request.AnswerId, cancellationToken);

        if (answer is null)
            return GenericResult<GenericResponse>.Failure(string.Format(EM.ANSWER_ID_NOTFOUND, request.QuestionId));

        if (!_authenticationContext.IsModerator())
        {
            return GenericResult<GenericResponse>.Failure("Need moderator role to accept this question");
        }

        if (answer.IsAccepted)
            return GenericResult<GenericResponse>.Failure("Answer is already been accepted");

        // Main logic
        question.IsSolved = true;
        answer.IsAccepted = true;
        answer.Author!.Reputation = _options.ReputationAcquirePerAction.AnswerAccepted;

        _questionHistoryRepository.AddHistory(question.Id, _authenticationContext.UserId, QuestionHistoryType.AcceptAnswer, answer.Content);

        var result = await _questionRepository.SaveChangesAsync(cancellationToken);

        await _questionSearchService.IndexOrUpdateAsync(question, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.Information("Answer with id {AnswerId} from question {QuestionId} is accepted by morderator {ModeratorId}", answer.Id, question.Id, _authenticationContext.UserId);
        }
        else
        {
            _logger.Information("Failed to accept answer with id {AnswerId} from question {QuestionId} by morderator {ModeratorId}", answer.Id, question.Id, _authenticationContext.UserId);
        }

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success(new("Done"))
            : GenericResult<GenericResponse>.Failure(result.Message);

    }
}
