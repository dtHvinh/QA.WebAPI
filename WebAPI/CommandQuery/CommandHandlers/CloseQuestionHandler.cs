using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CloseQuestionHandler(
    IQuestionRepository questionRepository,
    QuestionSearchService questionSearchService,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<CloseQuestionCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly QuestionSearchService _questionSearchService = questionSearchService;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<GenericResponse>> Handle(CloseQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var existQuestion =
            await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (existQuestion is null)
            return GenericResult<GenericResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        if (!_authenticationContext.IsModerator())
        {
            return GenericResult<GenericResponse>.Failure(
                string.Format(EM.ROLE_NOT_MEET_REQ, Roles.Moderator));
        }

        existQuestion.IsClosed = true;

        _questionRepository.UpdateQuestion(existQuestion);
        var result = await _questionRepository.SaveChangesAsync(cancellationToken);

        if (result.IsSuccess)
        {
            await _questionSearchService.IndexOrUpdateAsync(existQuestion, cancellationToken);
            _logger.Information("Question with id {QuestionId} is closed by moderator {ModeratorId}",
                existQuestion.Id, _authenticationContext.UserId);
        }
        else
        {
            _logger.Information("Failed to close question with id {QuestionId} by moderator {ModeratorId}",
                existQuestion.Id, _authenticationContext.UserId);
        }

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success("Question closed")
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}