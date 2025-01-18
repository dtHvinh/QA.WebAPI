using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Filters.Requirement;
using WebAPI.Filters.Validation;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Response.AsnwerResponses;
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
    }
}
