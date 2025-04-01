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
    : IRequestHandler<DeleteAnswerCommand, GenericResult<TextResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.FindFirstAsync(e => e.Id.Equals(request.Id), cancellationToken);
        if (answer is null)
            return GenericResult<TextResponse>.Failure("Answer not found");

        if (!_authContext.IsResourceOwnedByUser(answer))
        {
            if (await _authContext.IsModerator()) // Moderator delete
            {
                _answerRepository.Remove(answer);

                var morderatorDelRes = await _answerRepository.SaveChangesAsync(cancellationToken);

                _logger.ModeratorNoEnityOwnerAction(morderatorDelRes.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authContext.UserId, LogModeratorOp.Delete, answer);

                return morderatorDelRes.IsSuccess
                    ? GenericResult<TextResponse>.Success(new TextResponse("Answer deleted successfully"))
                    : GenericResult<TextResponse>.Failure(morderatorDelRes.Message);
            }
            else
            {
                return GenericResult<TextResponse>.Failure("You are not authorized to delete this answer");
            }
        }
        else // Answer owner delete
        {
            _answerRepository.TrySoftDeleteAnswer(answer, out var errMsg);

            if (errMsg is not null)
                return GenericResult<TextResponse>.Failure(errMsg);

            var result = await _answerRepository.SaveChangesAsync(cancellationToken);

            _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authContext.UserId, LogOp.Deleted, answer);

            return result.IsSuccess
                ? GenericResult<TextResponse>.Success(new TextResponse("Answer deleted successfully"))
                : GenericResult<TextResponse>.Failure(result.Message);
        }

    }
}
