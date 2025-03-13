using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Response;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Comment;

public class CommentModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Comment)
            .WithTags(nameof(CommentModule))
            .WithOpenApi();

        group.MapPut("/{id:int}", HandleUpdateComment)
            .WithName("UpdateComment")
            .WithSummary("Update a comment")
            .WithDescription("Updates the content of an existing comment. Only the comment owner can perform this operation.")
            .RequireAuthorization();

        group.MapDelete("/{id:int}", HandleDeleteComment)
            .WithName("DeleteComment")
            .WithSummary("Delete a comment")
            .WithDescription("Removes a comment from the system. Only the comment owner or moderators can perform this operation.")
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<CommentResponse>, ProblemHttpResult>> HandleUpdateComment(
        int id,
        [FromBody] UpdateCommentDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new UpdateCommentCommand(id, dto);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> HandleDeleteComment(
        int id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new DeleteCommentCommand(id);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }
}
