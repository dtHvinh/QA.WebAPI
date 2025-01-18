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
    : ICommandHandler<UpdateAnswerCommand, GenericResult<AnswerResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<AnswerResponse>> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.FindFirstAsync(e => e.Id.Equals(request.AnswerId), cancellationToken);
        if (answer == null)
        {
            return GenericResult<AnswerResponse>.Failure("Answer not found");
        }
        else if (!_authContext.IsResourceOwnedByUser(answer))
        {
            return GenericResult<AnswerResponse>.Failure("You are not authorized to update this answer");
        }

        answer.Content = request.Answer.NewContent;
        _answerRepository.Update(answer);

        var result = await _answerRepository.SaveChangesAsync(cancellationToken);
        return result.IsSuccess
            ? GenericResult<AnswerResponse>.Success(answer.ToAnswerResponse())
            : GenericResult<AnswerResponse>.Failure(result.Message);
    }
}
