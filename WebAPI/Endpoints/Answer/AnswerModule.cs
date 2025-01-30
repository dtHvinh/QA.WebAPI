using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Filters.Requirement;
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
        var group = endpoints.MapGroup(EG.Answer);

        group.MapPut("/{answerId:guid}",
            static async Task<Results<Ok<AnswerResponse>, ProblemHttpResult>> (
            [FromBody] UpdateAnswerDto dto,
            Guid answerId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new UpdateAnswerCommand(dto, answerId);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateAnswerDto>>()
            .AddEndpointFilter<AnswerReputationRequirement>();

        group.MapDelete("/{answerId:guid}",
            static async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            Guid answerId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new DeleteAnswerCommand(answerId);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<AnswerReputationRequirement>();

        group.MapPost("/{answerId:guid}/{action:regex(^(upvote|downvote)$)}",
            async Task<Results<Ok<VoteResponse>, ProblemHttpResult>> (
            Guid answerId,
            string action,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new CreateAnswerVoteCommand(answerId, action == "upvote");

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<UpvoteAndDownvoteReputationRequirement>();
    }
}
