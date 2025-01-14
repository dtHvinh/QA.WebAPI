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
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authentcationContext = authentcationContext;

    public async Task<OperationResult<CreateQuestionResponse>> Handle(
        CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        // Add question 
        var question = request.Question.ToQuestion(_authentcationContext.UserId);
        _questionRepository.Add(question);

        // Add tags to question
        var tagIds = request.Question.Tags.Select(e => e.Id).ToList();
        _tagRepository.AddQuestionToTags(question, tagIds);

        var opResult = await _questionRepository.SaveChangesAsync(cancellationToken);
        if (!opResult.IsSuccess)
        {
            return OperationResult<CreateQuestionResponse>.Failure(opResult.Message);
        }

        var response = new CreateQuestionResponse(Id: question.Id,
                                                  Title: question.Title,
                                                  Content: question.Content,
                                                  TagObjects: request.Question.Tags);

        return OperationResult<CreateQuestionResponse>.Success(response);
    }
}
