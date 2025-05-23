using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class ReopenQuestionHandler(
    IQuestionRepository questionRepository,
    AuthenticationContext authenticationContext,
    IQuestionHistoryRepository historyRepository,
    Serilog.ILogger logger)
    : ICommandHandler<ReopenQuestionCommand, GenericResult<TextResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly IQuestionHistoryRepository _historyRepository = historyRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(ReopenQuestionCommand request, CancellationToken cancellationToken)
    {
        var existQuestion =
            await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (existQuestion is null)
            return GenericResult<TextResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        if (!await _authenticationContext.IsModerator())
        {
            return GenericResult<TextResponse>.Failure(
                string.Format(EM.ROLE_NOT_MEET_REQ, Roles.Moderator));
        }

        existQuestion.IsClosed = false;

        await _questionRepository.UpdateQuestion(existQuestion);

        await _historyRepository.AddHistory(existQuestion.Id,
            _authenticationContext.UserId, QuestionHistoryTypes.Reopen, "", cancellationToken);

        var result = await _questionRepository.SaveChangesAsync(cancellationToken);


        _logger.ModeratorNoEnityOwnerAction(result.IsSuccess
            ? LogEventLevel.Information
            : LogEventLevel.Error, _authenticationContext.UserId, LogModeratorOp.Reopen, existQuestion);

        return result.IsSuccess
            ? GenericResult<TextResponse>.Success("Question closed")
            : GenericResult<TextResponse>.Failure(result.Message);
    }
}
