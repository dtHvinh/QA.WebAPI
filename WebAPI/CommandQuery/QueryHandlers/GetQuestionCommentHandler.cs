using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionCommentHandler(ICommentRepository commentRepository, AuthenticationContext authenticationContext)
    : IQueryHandler<GetQuestionCommentQuery, GenericResult<List<CommentResponse>>>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<List<CommentResponse>>> Handle(GetQuestionCommentQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentRepository.GetQuestionCommentWithAuthor(request.QuestionId, request.PageArgs.CalculateSkip(), request.PageArgs.PageSize, cancellationToken);

        return GenericResult<List<CommentResponse>>.Success(comments.Select(e => e.ToCommentResponse().SetResourceRight(_authenticationContext.UserId)).ToList());
    }
}
