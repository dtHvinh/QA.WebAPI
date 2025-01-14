using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Response;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Question;

public sealed class QuestionModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Question);

        group.MapPost("/", async Task<Results<Ok<CreateQuestionResponse>, ProblemHttpResult>> (
            [FromBody] CreateQuestionDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var cmd = new CreateQuestionCommand(dto);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateQuestionDto>>();

        group.MapDelete("/{id:guid}", async Task<Results<Ok<DeleteQuestionResponse>, ProblemHttpResult>> (
            Guid id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var cmd = new DeleteQuestionCommand(id);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
             .RequireAuthorization();

        group.MapPut("/", async Task<Results<Ok<UpdateQuestionResponse>, ProblemHttpResult>> (
            [FromBody] UpdateQuestionDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var cmd = new UpdateQuestionCommand(dto);
            var result = await mediator.Send(cmd, cancellationToken);
            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }
            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateQuestionDto>>();
    }
}
