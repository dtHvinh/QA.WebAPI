using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateQuestionHandler(IQuestionRepository questionRepository,
                                   IQuestionHistoryRepository questionHistoryRepository,
                                   ITagRepository tagRepository,
                                   AuthenticationContext authenticationContext,
                                   QuestionSearchService questionSearchService,
                                   Serilog.ILogger logger)
    : ICommandHandler<UpdateQuestionCommand, GenericResult<UpdateQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IQuestionHistoryRepository _questionHistoryRepository = questionHistoryRepository;
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly QuestionSearchService _questionSearchService = questionSearchService;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<UpdateQuestionResponse>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        // Check conditions
        var existQuestion = await _questionRepository.FindQuestionWithTags(request.UpdateObject.Id, cancellationToken);

        if (existQuestion == null)
        {
            return GenericResult<UpdateQuestionResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND
                , request.UpdateObject.Id));
        }

        if (!_authenticationContext.IsResourceOwnedByUser(existQuestion))
        {
            return GenericResult<UpdateQuestionResponse>.Failure(EM.ACTION_REQUIRE_RESOURCE_OWNER);
        }

        if (existQuestion.Upvotes != 0 || existQuestion.Downvotes != 0)
        {
            return GenericResult<UpdateQuestionResponse>.Failure("Can not edit question with votes");
        }

        if (existQuestion.IsSolved)
        {
            return GenericResult<UpdateQuestionResponse>.Failure("Can not edit solved question");
        }

        // Logic
        var tagsToUpdate = await _tagRepository.FindAllTagByIds(request.UpdateObject.Tags, cancellationToken);

        existQuestion.FromUpdateObject(request.UpdateObject);

        foreach (var tag in tagsToUpdate.Except(existQuestion.Tags))
        {
            tag.QuestionCount++;
        }

        foreach (var tag in existQuestion.Tags.Except(tagsToUpdate))
        {
            tag.QuestionCount--;
        }

        existQuestion.Tags = tagsToUpdate;

        _questionRepository.UpdateQuestion(existQuestion);
        _tagRepository.UpdateRange(tagsToUpdate);

        _questionHistoryRepository.AddHistory(existQuestion.Id, _authenticationContext.UserId, QuestionHistoryType.Edit, request.UpdateObject.Comment);

        var updateOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        var indexDoc = await _questionSearchService.IndexOrUpdateAsync(existQuestion, cancellationToken);
        if (!indexDoc)
        {
            return GenericResult<UpdateQuestionResponse>.Failure(EM.ES_INDEX_OR_UPDATE_DOCUMENT_FAILED);
        }

        _logger.UserAction(updateOp.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Updated, existQuestion);

        return updateOp.IsSuccess
            ? GenericResult<UpdateQuestionResponse>.Success(
                new UpdateQuestionResponse("Question updated successfully."))
            : GenericResult<UpdateQuestionResponse>.Failure("Failed to update question.");
    }
}
