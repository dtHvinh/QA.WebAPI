using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateAnswerHandler(IAnswerRepository answerRepository,
                                 AuthenticationContext authContext,
                                 Serilog.ILogger logger)
    : ICommandHandler<UpdateAnswerCommand, GenericResult<AnswerResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly Serilog.ILogger _logger = logger;

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

        answer.Content = request.Answer.Content;

        _answerRepository.TryEditAnswer(answer, out var errMsg);

        if (errMsg is not null)
            return GenericResult<AnswerResponse>.Failure(errMsg);

        var result = await _answerRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authContext.UserId, LogOp.Updated, answer);

        return result.IsSuccess
            ? GenericResult<AnswerResponse>.Success(answer.ToAnswerResponse())
            : GenericResult<AnswerResponse>.Failure(result.Message);
    }
}
