using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class RemoveDuplicateFlagHandler(
    IQuestionHistoryRepository questionHistoryRepository,
    IQuestionRepository questionRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<RemoveDuplicateFlagCommand, GenericResult<TextResponse>>
{
    private readonly IQuestionHistoryRepository _questionHistoryRepository = questionHistoryRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(RemoveDuplicateFlagCommand request, CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsModerator())
            return GenericResult<TextResponse>.Failure("You are not a moderator.");

        var question = await _questionRepository.FindQuestionById(request.QuestionId, cancellationToken);

        if (question is null)
            return GenericResult<TextResponse>.Failure("Question not found.");

        question.IsDuplicate = false;
        question.DuplicateQuestionUrl = string.Empty;

        await _questionRepository.UpdateQuestion(question);

        await _questionHistoryRepository.AddHistory(
            questionId: question.Id,
            authorId: _authenticationContext.UserId,
            questionHistoryType: QuestionHistoryTypes.RemoveDuplicateFlag,
            comment: "Removed duplicate flag.");

        var res = await _questionRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error,
            _authenticationContext.UserId, LogOp.RemoveDuplicateFlag, question);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("Duplicate flag removed.")
            : GenericResult<TextResponse>.Failure("Failed to remove duplicate flag.");
    }
}
