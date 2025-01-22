using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateCommentHandler(ICommentRepository commentRepository, AuthenticationContext authenticationContext)
    : ICommandHandler<UpdateCommentCommand, GenericResult<CommentResponse>>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<CommentResponse>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(request.Id);
        if (comment == null)
        {
            return (GenericResult<CommentResponse>.Failure("Comment not found"));
        }

        if (!_authenticationContext.IsResourceOwnedByUser(comment))
        {
            return GenericResult<CommentResponse>.Failure("You are not authorized to delete this comment");
        }

        _commentRepository.UpdateComment(comment.UpdateFrom(request.Comment));

        var result = await _commentRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<CommentResponse>.Success(comment.ToCommentResponse())
            : GenericResult<CommentResponse>.Failure("Failed to update comment");
    }
}
