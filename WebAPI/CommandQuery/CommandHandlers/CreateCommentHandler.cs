using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCommentHandler(ICommentRepository commentRepository,
                                  IQuestionRepository questionRepository,
                                  AuthenticationContext authContext)
    : ICommandHandler<CreateCommentCommand, GenericResult<CommentResponse>>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<CommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (request.CommentType == Model.CommentTypes.Question)
        {
            var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.ObjectId, cancellationToken);

            if (question is null)
                return GenericResult<CommentResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.ObjectId));

            if (question.IsClosed)
                return GenericResult<CommentResponse>.Failure(EM.QUESTION_CLOSED_COMMENT_RESTRICT);

        }

        var newComment = request.Comment.ToComment(request.CommentType, _authContext.UserId, request.ObjectId);
        _commentRepository.Add(newComment);

        var res = await _commentRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? GenericResult<CommentResponse>.Success(
                newComment.ToCommentResponse().SetResourceRight(_authContext.UserId))
            : GenericResult<CommentResponse>.Failure(res.Message);
    }
}
