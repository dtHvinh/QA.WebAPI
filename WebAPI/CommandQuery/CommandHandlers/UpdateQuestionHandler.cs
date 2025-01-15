using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateQuestionHandler(IQuestionRepository questionRepository,
                                   ITagRepository tagRepository)
    : ICommandHandler<UpdateQuestionCommand, OperationResult<UpdateQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<OperationResult<UpdateQuestionResponse>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var existQuestion = await _questionRepository.FindFirstAsync(
            e => e.Id.Equals(request.Question.Id), cancellationToken);
        if (existQuestion == null)
            return OperationResult<UpdateQuestionResponse>.Failure(
                string.Format(EM.QUESTION_ID_NOTFOUND, request.Question.Id));

        _tagRepository.AddQuestionToTags(existQuestion, request.Question.AdditionalTags);
        existQuestion.Update(request.Question);

        _questionRepository.Update(existQuestion);
        var updateOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        return updateOp.IsSuccess
            ? OperationResult<UpdateQuestionResponse>.Success(
                new UpdateQuestionResponse("Question updated successfully."))
            : OperationResult<UpdateQuestionResponse>.Failure("Failed to update question.");
    }
}
