using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteQuestionHandler(IQuestionRepository questionRepository,
                                   ITagRepository tagRepository,
                                   AuthenticationContext authContext,
                                   IAnswerRepository answerRepository,
                                   QuestionSearchService questionSearchService,
                                   Serilog.ILogger logger) :
    ICommandHandler<DeleteQuestionCommand, GenericResult<DeleteQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly QuestionSearchService _questionSearchService = questionSearchService;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<DeleteQuestionResponse>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var questionToDelete = await _questionRepository.FindFirstAsync(
            e => e.Id.Equals(request.Id), cancellationToken);

        if (questionToDelete is null)
            return GenericResult<DeleteQuestionResponse>.Failure(
                string.Format(EM.QUESTION_ID_NOTFOUND, request.Id));

        if (!_authContext.IsModerator())
            return GenericResult<DeleteQuestionResponse>.Failure(string.Format(EM.ROLE_NOT_MEET_REQ, Roles.Moderator));

        string? errMessage = null;

        if (questionToDelete.IsSolved)
        {
            errMessage = "Can not delete solved question";
        }

        if (questionToDelete.Score > 0)
        {
            errMessage = "Can not delete question people may find it valuable";
        }

        var allAnswer = await _answerRepository.GetAnswersAsync(questionToDelete.Id, cancellationToken);

        if (allAnswer.Any(e => e.Score > 0))
        {
            errMessage = "Can not delete question people may find it valuable";
        }

        if (errMessage is not null)
            return GenericResult<DeleteQuestionResponse>.Failure(errMessage);

        var allTags = await _tagRepository.GetQuestionTags(questionToDelete, cancellationToken);

        foreach (var tag in allTags)
        {
            tag.QuestionCount--;
        }

        _tagRepository.UpdateRange(allTags);
        _questionRepository.SoftDeleteQuestion(questionToDelete);

        var delOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(delOp.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authContext.UserId, LogOp.Deleted, questionToDelete);

        await _questionSearchService.IndexOrUpdateAsync(questionToDelete, cancellationToken);

        return delOp.IsSuccess
            ? GenericResult<DeleteQuestionResponse>.Success(new DeleteQuestionResponse(request.Id))
            : GenericResult<DeleteQuestionResponse>.Failure(delOp.Message);
    }
}
