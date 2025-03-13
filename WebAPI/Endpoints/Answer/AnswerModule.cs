using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Response;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Response.VoteResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Answer;

public class AnswerModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Answer)
                           .WithTags(nameof(AnswerModule))
                           .WithOpenApi();

        group.MapPut("/{answerId:int}", HandleUpdateAnswer)
            .WithName("UpdateAnswer")
            .WithSummary("Update an existing answer")
            .WithDescription("Updates the content of an existing answer. Only the answer owner can perform this operation.")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateAnswerDto>>();

        group.MapDelete("/{answerId:int}", HandleDeleteAnswer)
            .WithName("DeleteAnswer")
            .WithSummary("Delete an answer")
            .WithDescription("Removes an answer from the system. Only the answer owner or moderators can perform this operation.")
            .RequireAuthorization();

        group.MapPost("/{answerId:int}/{action:regex(^(upvote|downvote)$)}", HandleVoteAnswer)
            .WithName("VoteAnswer")
            .WithSummary("Vote on an answer")
            .WithDescription("Allows users to upvote or downvote an answer. Users can only vote once, but can change their vote.")
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<AnswerResponse>, ProblemHttpResult>> HandleUpdateAnswer(
        [FromBody] UpdateAnswerDto dto,
        int answerId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new UpdateAnswerCommand(dto, answerId);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> HandleDeleteAnswer(
        int answerId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new DeleteAnswerCommand(answerId);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<VoteResponse>, ProblemHttpResult>> HandleVoteAnswer(
        int answerId,
        string action,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new CreateAnswerVoteCommand(answerId, action == "upvote");
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }
}
