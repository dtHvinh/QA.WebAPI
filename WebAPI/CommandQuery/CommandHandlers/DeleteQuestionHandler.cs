using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteQuestionHandler(IQuestionRepository questionRepository,
                                   AuthenticationContext authContext) :
    ICommandHandler<DeleteQuestionCommand, OperationResult<DeleteQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<OperationResult<DeleteQuestionResponse>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var questionToDelete = await _questionRepository.FindFirstAsync(
            e => e.Id.Equals(request.Id), cancellationToken);

        if (questionToDelete is null)
            return OperationResult<DeleteQuestionResponse>.Failure(
                string.Format(EM.QUESTION_ID_NOTFOUND, request.Id));

        if (_authContext.UserId != questionToDelete.AuthorId)
            return OperationResult<DeleteQuestionResponse>.Failure(EM.QUESTION_DELETE_UNAUTHORIZED);

        _questionRepository.Remove(questionToDelete);
        var delOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        return delOp.IsSuccess
            ? OperationResult<DeleteQuestionResponse>.Success(new DeleteQuestionResponse(request.Id))
            : OperationResult<DeleteQuestionResponse>.Failure(delOp.Message);
    }
}
