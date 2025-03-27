using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class FlagQuestionDuplicateHandler(
    AuthenticationContext authenticationContext,
    IQuestionRepository questionRepository,
    IQuestionHistoryRepository questionHistoryRepository,
    Serilog.ILogger logger)
    : ICommandHandler<FlagQuestionDuplicateCommand, GenericResult<TextResponse>>
{
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly IQuestionRepository questionRepository = questionRepository;
    private readonly IQuestionHistoryRepository _questionHistoryRepository = questionHistoryRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(FlagQuestionDuplicateCommand request, CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsModerator())
            return GenericResult<TextResponse>.Failure("You are not a moderator.");

        var question = await questionRepository.FindQuestionById(request.QuestionId, cancellationToken);

        if (question is null)
            return GenericResult<TextResponse>.Failure("Question not found.");

        question.IsDuplicate = true;
        question.DuplicateQuestionUrl = request.DuplicateQuestionUrl;

        await questionRepository.UpdateQuestion(question);

        _questionHistoryRepository.AddHistory(
            questionId: question.Id,
            authorId: _authenticationContext.UserId,
            questionHistoryType: QuestionHistoryTypes.FlagDuplicate,
            comment: $"Flagged as duplicate of {MarkupText.Link(request.DuplicateQuestionUrl, request.DuplicateQuestionUrl)}");

        var res = await questionRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error,
            _authenticationContext.UserId, LogOp.FlagQuestionDuplicated, question);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success(request.DuplicateQuestionUrl)
            : GenericResult<TextResponse>.Failure("Failed to flag question as duplicate.");
    }
}
