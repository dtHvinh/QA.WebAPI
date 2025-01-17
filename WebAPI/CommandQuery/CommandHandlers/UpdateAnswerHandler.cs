using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response.AsnwerResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateAnswerHandler(IAnswerRepository answerRepository,
                                 AuthenticationContext authContext)
    : ICommandHandler<UpdateAnswerCommand, OperationResult<AnswerResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<OperationResult<AnswerResponse>> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.FindFirstAsync(e => e.Id.Equals(request.AnswerId), cancellationToken);
        if (answer == null)
        {
            return OperationResult<AnswerResponse>.Failure("Answer not found");
        }
        else if (answer.AuthorId != _authContext.UserId)
        {
            return OperationResult<AnswerResponse>.Failure("You are not authorized to update this answer");
        }

        answer.Content = request.Answer.NewContent;
        _answerRepository.Update(answer);

        var result = await _answerRepository.SaveChangesAsync(cancellationToken);
        return result.IsSuccess
            ? OperationResult<AnswerResponse>.Success(answer.ToAnswerResponse())
            : OperationResult<AnswerResponse>.Failure(result.Message);
    }
}
