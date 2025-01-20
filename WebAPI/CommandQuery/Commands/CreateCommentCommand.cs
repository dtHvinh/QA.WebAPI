using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateCommentCommand(CreateCommentDto Comment, CommentTypes CommentType, Guid ObjectId)
    : ICommand<GenericResult<CommentResponse>>;
