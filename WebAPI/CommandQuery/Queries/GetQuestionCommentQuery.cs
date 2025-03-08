using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetQuestionCommentQuery(int QuestionId, PageArgs PageArgs) : IQuery<GenericResult<List<CommentResponse>>>;
