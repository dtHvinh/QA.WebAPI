using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateCommentHandler(ICommentRepository commentRepository,
                                  IQuestionRepository questionRepository,
                                  AuthenticationContext authContext,
                                  IQuestionHistoryRepository questionHistoryRepository,
                                  Serilog.ILogger logger)
    : ICommandHandler<CreateCommentCommand, GenericResult<CommentResponse>>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly IQuestionHistoryRepository _questionHistoryRepository = questionHistoryRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<CommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (request.CommentType == CommentTypes.Question)
        {
            var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.ObjectId, cancellationToken);

            if (question is null)
                return GenericResult<CommentResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.ObjectId));

            if (question.IsClosed)
                return GenericResult<CommentResponse>.Failure(EM.QUESTION_CLOSED_COMMENT_RESTRICT);

            _questionHistoryRepository.AddHistory(
                question.Id, _authContext.UserId, QuestionHistoryType.AddComment, request.Comment.Content);
        }

        var newComment = request.Comment.ToComment(request.CommentType, _authContext.UserId, request.ObjectId);
        _commentRepository.Add(newComment);
        var res = await _commentRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authContext.UserId, LogOp.Created, newComment);

        return res.IsSuccess
            ? GenericResult<CommentResponse>.Success(
                newComment.ToCommentResponse().SetResourceRight(_authContext.UserId))
            : GenericResult<CommentResponse>.Failure(res.Message);
    }
}
