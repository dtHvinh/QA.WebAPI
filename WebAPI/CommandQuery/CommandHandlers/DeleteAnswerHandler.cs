using MediatR;
using WebAPI.CommandQuery.Commands;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteAnswerHandler(IAnswerRepository answerRepository, AuthenticationContext authContext)
    : IRequestHandler<DeleteAnswerCommand, GenericResult<GenericResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<GenericResponse>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.FindFirstAsync(e => e.Id.Equals(request.Id), cancellationToken);
        if (answer is null)
            return GenericResult<GenericResponse>.Failure("Answer not found");

        if (!_authContext.IsResourceOwnedByUser(answer))
            return GenericResult<GenericResponse>.Failure("You are not authorized to delete this answer");

        _answerRepository.Remove(answer);
        var result = await _answerRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success(new GenericResponse("Answer deleted successfully"))
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
