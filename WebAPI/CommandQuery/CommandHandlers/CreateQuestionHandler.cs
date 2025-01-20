using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateQuestionHandler(AuthenticationContext authentcationContext,
                                   IQuestionRepository questionRepository,
                                   ITagRepository tagRepository)
    : ICommandHandler<CreateQuestionCommand, GenericResult<CreateQuestionResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authentcationContext = authentcationContext;

    public async Task<GenericResult<CreateQuestionResponse>> Handle(
        CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        // Add question
        var question = request.Question.ToQuestion(_authentcationContext.UserId, request.DraftMode);

        var tags = await _tagRepository.FindAllTagByIds(request.Question.Tags, cancellationToken);
        question.Tags = tags;

        _questionRepository.Add(question);

        // Process result
        var opResult = await _questionRepository.SaveChangesAsync(cancellationToken);
        if (!opResult.IsSuccess)
        {
            return GenericResult<CreateQuestionResponse>.Failure(opResult.Message);
        }

        var response = new CreateQuestionResponse(Id: question.Id,
                                                  Title: question.Title,
                                                  Content: question.Content,
                                                  Tags: request.Question.Tags);

        return GenericResult<CreateQuestionResponse>.Success(response);
    }
}
