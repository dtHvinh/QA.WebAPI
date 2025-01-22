using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteCommentHandler(ICommentRepository commentRepository, AuthenticationContext authenticationContext)
    : ICommandHandler<DeleteCommentCommand, GenericResult<GenericResponse>>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(request.Id);
        if (comment == null)
        {
            return (GenericResult<GenericResponse>.Failure("Comment not found"));
        }
        if (!_authenticationContext.IsResourceOwnedByUser(comment))
        {
            return GenericResult<GenericResponse>.Failure("You are not authorized to delete this comment");
        }

        _commentRepository.Remove(comment);

        var result = await _commentRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success(new GenericResponse("Comment deleted successfully"))
            : GenericResult<GenericResponse>.Failure("Failed to delete comment");
    }
}
