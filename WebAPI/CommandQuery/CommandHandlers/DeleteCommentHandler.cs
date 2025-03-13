using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteCommentHandler(
    ICommentRepository commentRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<DeleteCommentCommand, GenericResult<TextResponse>>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(request.Id);
        if (comment == null)
        {
            return (GenericResult<TextResponse>.Failure("Comment not found"));
        }
        if (!_authenticationContext.IsResourceOwnedByUser(comment))
        {
            return GenericResult<TextResponse>.Failure("You are not authorized to delete this comment");
        }

        _commentRepository.Remove(comment);

        var result = await _commentRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Deleted, comment);

        return result.IsSuccess
            ? GenericResult<TextResponse>.Success(new TextResponse("Comment deleted successfully"))
            : GenericResult<TextResponse>.Failure("Failed to delete comment");
    }
}
