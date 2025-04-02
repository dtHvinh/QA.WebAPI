using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class ModDeleteQuestionHandler(
    IQuestionRepository questionRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<ModDeleteQuestionCommand, GenericResult<TextResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(ModDeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsModerator())
        {
            return GenericResult<TextResponse>.Failure("You are not moderator");
        }

        var question = await _questionRepository.FindQuestionById(request.QuestionId, cancellationToken);

        if (question == null)
        {
            return GenericResult<TextResponse>.Failure("Question not found");
        }

        _questionRepository.SoftDeleteQuestion(question);

        var res = await _questionRepository.SaveChangesAsync(cancellationToken);

        _logger.ModeratorNoEnityOwnerAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId, LogModeratorOp.Delete, question);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success(new TextResponse("Question deleted"))
            : GenericResult<TextResponse>.Failure("Failed to delete question");
    }
}
