using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
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
        var existQuestion = await _questionRepository.FindFirstAsync(
            e => e.Id.Equals(request.UpdateObject.Id), cancellationToken);

        if (existQuestion == null)
        {
            return GenericResult<UpdateQuestionResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND
                , request.UpdateObject.Id));
        }

        if (existQuestion.IsSolved)
        {
            return GenericResult<UpdateQuestionResponse>.Failure("Can not edit solved question");
        }

        var tags = await _tagRepository.FindAllTagByIds(request.UpdateObject.Tags, cancellationToken);

        existQuestion.FromUpdateObject(request.UpdateObject);

        await _questionRepository.SetQuestionTag(existQuestion, tags);

        _questionRepository.UpdateQuestion(existQuestion);

        _questionHistoryRepository.AddHistory(existQuestion.Id, _authenticationContext.UserId, QuestionHistoryType.Edit, request.UpdateObject.Comment ?? string.Empty);

        var updateOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        var indexDoc = await _questionSearchService.IndexOrUpdateAsync(existQuestion, cancellationToken);
        if (!indexDoc)
        {
            return GenericResult<UpdateQuestionResponse>.Failure(EM.ES_INDEX_OR_UPDATE_DOCUMENT_FAILED);
        }

        if (updateOp.IsSuccess)
            _logger.Information("Question with id: {QuestionId} updated", existQuestion.Id);

        return updateOp.IsSuccess
            ? GenericResult<UpdateQuestionResponse>.Success(
                new UpdateQuestionResponse("Question updated successfully."))
            : GenericResult<UpdateQuestionResponse>.Failure("Failed to update question.");
    }
}
