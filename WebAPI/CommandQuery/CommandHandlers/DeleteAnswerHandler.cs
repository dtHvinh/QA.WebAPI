using MediatR;
using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteAnswerHandler(
    IAnswerRepository answerRepository, AuthenticationContext authContext, Serilog.ILogger logger)
    : IRequestHandler<DeleteAnswerCommand, GenericResult<GenericResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<GenericResponse>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.FindFirstAsync(e => e.Id.Equals(request.Id), cancellationToken);
        if (answer is null)
            return GenericResult<GenericResponse>.Failure("Answer not found");

        if (!_authContext.IsResourceOwnedByUser(answer) && !_authContext.IsModerator())
        {
            return GenericResult<GenericResponse>.Failure("You are not authorized to delete this answer");
        }

        _answerRepository.TrySoftDeleteAnswer(answer, out var errMsg);

        if (errMsg is not null)
            return GenericResult<GenericResponse>.Failure(errMsg);

        var result = await _answerRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authContext.UserId, LogOp.Deleted, answer);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success(new GenericResponse("Answer deleted successfully"))
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
