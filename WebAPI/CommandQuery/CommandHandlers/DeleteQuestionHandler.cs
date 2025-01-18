using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteQuestionHandler(IQuestionRepository questionRepository,
                                   AuthenticationContext authContext) :
    ICommandHandler<DeleteQuestionCommand, GenericResult<DeleteQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<DeleteQuestionResponse>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var questionToDelete = await _questionRepository.FindFirstAsync(
            e => e.Id.Equals(request.Id), cancellationToken);

        if (questionToDelete is null)
            return GenericResult<DeleteQuestionResponse>.Failure(
                string.Format(EM.QUESTION_ID_NOTFOUND, request.Id));

        if (!_authContext.IsResourceOwnedByUser(questionToDelete))
            return GenericResult<DeleteQuestionResponse>.Failure(EM.QUESTION_DELETE_UNAUTHORIZED);

        _questionRepository.Remove(questionToDelete);
        var delOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        return delOp.IsSuccess
            ? GenericResult<DeleteQuestionResponse>.Success(new DeleteQuestionResponse(request.Id))
            : GenericResult<DeleteQuestionResponse>.Failure(delOp.Message);
    }
}
