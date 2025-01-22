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
        var group = endpoints.MapGroup(EG.Comment);

        group.MapPut("/{id:guid}", async Task<Results<Ok<CommentResponse>, ProblemHttpResult>> (
            Guid id,
            [FromBody] UpdateCommentDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
                {
                    var cmd = new UpdateCommentCommand(id, dto);
                    var result = await mediator.Send(cmd, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        return ProblemResultExtensions.BadRequest(result.Message);
                    }
                    return TypedResults.Ok(result.Value);
                })
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            Guid id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new DeleteCommentCommand(id);
            var result = await mediator.Send(cmd, cancellationToken);
            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }
            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();
    }
}
