using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateQuestionHandler(IQuestionRepository questionRepository,
                                   IUserRepository userRepository,
                                   ITagRepository tagRepository,
                                   AuthenticationContext authenticationContext,
                                   IOptions<ApplicationProperties> applicationProperties,
                                   QuestionSearchService questionSearchService)
    : ICommandHandler<UpdateQuestionCommand, GenericResult<UpdateQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly ApplicationProperties _applicationProperties = applicationProperties.Value;
    private readonly QuestionSearchService _questionSearchService = questionSearchService;

    public async Task<GenericResult<UpdateQuestionResponse>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var existQuestion = await _questionRepository.FindFirstAsync(
            e => e.Id.Equals(request.Question.Id), cancellationToken);

        if (existQuestion == null)
        {
            return GenericResult<UpdateQuestionResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND
                , request.Question.Id));
        }

        if (!_authenticationContext.IsAdmin())
        {
            return GenericResult<UpdateQuestionResponse>.Failure("Need admin role to delete this question");
        }

        if (existQuestion.IsSolved)
        {
            return GenericResult<UpdateQuestionResponse>.Failure("Can not edit solved question");
        }

        var tags = await _tagRepository.FindAllTagByIds(request.Question.Tags, cancellationToken);

        existQuestion.FromUpdateObject(request.Question);

        await _questionRepository.SetQuestionTag(existQuestion, tags);

        if (existQuestion.IsDraft)
            existQuestion.IsDraft = false;

        _questionRepository.UpdateQuestion(existQuestion);

        var updateOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        var indexDoc = await _questionSearchService.IndexOrUpdateAsync(existQuestion, cancellationToken);
        if (!indexDoc)
        {
            return GenericResult<UpdateQuestionResponse>.Failure(EM.ES_INDEX_OR_UPDATE_DOCUMENT_FAILED);
        }

        return updateOp.IsSuccess
            ? GenericResult<UpdateQuestionResponse>.Success(
                new UpdateQuestionResponse("Question updated successfully."))
            : GenericResult<UpdateQuestionResponse>.Failure("Failed to update question.");
    }
}
