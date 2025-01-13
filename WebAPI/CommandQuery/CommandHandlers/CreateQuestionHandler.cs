using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateQuestionHandler(AuthenticationContext authentcationContext,
                                   IQuestionRepository questionRepository,
                                   ITagRepository tagRepository)
    : ICommandHandler<CreateQuestionCommand, OperationResult<CreateQuestionResponse>>
{
    private readonly AuthenticationContext _authentcationContext = authentcationContext;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<OperationResult<CreateQuestionResponse>> Handle(
        CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = request.Question.ToQuestion(_authentcationContext.UserId);

        var addQuestionResult = await _questionRepository.AddQuestionAsync(question, cancellationToken);
        if (!addQuestionResult.IsSuccess)
        {
            return OperationResult<CreateQuestionResponse>.Failure(addQuestionResult.Message);
        }

        var tagIds = request.Question.TagObjects.Select(e => e.Id).ToList();

        var addTagToQuestionReult = await _tagRepository.AddQuestionToTagsAsync(
            question, tagIds, cancellationToken);

        if (!addTagToQuestionReult.IsSuccess)
        {
            return OperationResult<CreateQuestionResponse>.Failure(addTagToQuestionReult.Message);
        }

        var response = new CreateQuestionResponse(Id: addQuestionResult.Value!.Id,
                                                  Title: addQuestionResult.Value.Title,
                                                  Content: addQuestionResult.Value.Content,
                                                  TagObjects: request.Question.TagObjects);

        return OperationResult<CreateQuestionResponse>.Success(response);
    }
}
