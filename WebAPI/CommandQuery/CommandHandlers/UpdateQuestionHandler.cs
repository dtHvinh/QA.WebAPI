using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateQuestionHandler(IQuestionRepository questionRepository,
                                   AuthenticationContext authContext)
    : ICommandHandler<UpdateQuestionCommand, OperationResult<UpdateQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<OperationResult<UpdateQuestionResponse>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var newQuestion = request.Question.ToQuestion(_authContext.UserId);
        _questionRepository.Update(newQuestion);

        var updateOp = await _questionRepository.SaveChangesAsync(cancellationToken);

        return updateOp.IsSuccess
            ? OperationResult<UpdateQuestionResponse>.Success(
                new UpdateQuestionResponse("Question updated successfully."))
            : OperationResult<UpdateQuestionResponse>.Failure("Failed to update question.");
    }
}
