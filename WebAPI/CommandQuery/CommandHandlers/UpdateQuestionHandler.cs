using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateQuestionHandler(IQuestionRepository questionRepository,
                                   ITagRepository tagRepository,
                                   AuthenticationContext authenticationContext)
    : ICommandHandler<UpdateQuestionCommand, GenericResult<UpdateQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<UpdateQuestionResponse>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var existQuestion = await _questionRepository.FindFirstAsync(
            e => e.Id.Equals(request.Question.Id), cancellationToken);

        if (existQuestion == null)
        {
            return GenericResult<UpdateQuestionResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND
                , request.Question.Id));
        }

        if (!_authenticationContext.IsResourceOwnedByUser(existQuestion))
        {
            return GenericResult<UpdateQuestionResponse>.Failure("You have not authorized to perform this action");
        }

        if (existQuestion.IsSolved)
        {
            return GenericResult<UpdateQuestionResponse>.Failure("Can not edit solved question");
        }

        if (existQuestion.Upvote > existQuestion.Downvote)
        {
            return GenericResult<UpdateQuestionResponse>.Failure("Can not edit question people have upvoted");
        }

        var tags = await _tagRepository.FindAllTagByIds(request.Question.Tags, cancellationToken);

        existQuestion.FromUpdateObject(request.Question);

        await _questionRepository.SetQuestionTag(existQuestion, tags);

        if (existQuestion.IsDraft)
            existQuestion.IsDraft = false;

        _questionRepository.UpdateQuestion(existQuestion);

        var updateOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        return updateOp.IsSuccess
            ? GenericResult<UpdateQuestionResponse>.Success(
                new UpdateQuestionResponse("Question updated successfully."))
            : GenericResult<UpdateQuestionResponse>.Failure("Failed to update question.");
    }
}
