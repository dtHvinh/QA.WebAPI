using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response.CommentResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCommentHandler(ICommentRepository commentRepository, AuthenticationContext authContext)
    : ICommandHandler<CreateCommentCommand, OperationResult<CommentResponse>>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<OperationResult<CommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var newComment = request.Comment.ToComment(request.CommentType, _authContext.UserId, request.ObjectId);
        _commentRepository.Add(newComment);

        var res = await _commentRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? OperationResult<CommentResponse>.Success(newComment.ToCommentResponse())
            : OperationResult<CommentResponse>.Failure(res.Message);
    }
}
