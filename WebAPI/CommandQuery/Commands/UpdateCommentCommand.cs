using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record UpdateCommentCommand(int Id, UpdateCommentDto Comment) : ICommand<GenericResult<CommentResponse>>;
