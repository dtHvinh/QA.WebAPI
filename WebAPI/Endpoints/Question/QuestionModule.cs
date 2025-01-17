using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Pagination;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Response.QuestionResponses;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Question;

public sealed class QuestionModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Question);

        #region GET
        group.MapGet("/view/{id:guid}", async Task<Results<Ok<GetQuestionResponse>, ProblemHttpResult>> (
            Guid id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new GetSingleQuestionQuery(id);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapGet("/search",
            async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> (
            [FromQuery] string keyword,
            [FromQuery] Guid tagId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new SearchQuestionQuery(keyword, tagId, PageArgs.From(page, pageSize));

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        #endregion GET

        #region POST

        group.MapPost("/", async Task<Results<Ok<CreateQuestionResponse>, ProblemHttpResult>> (
            [FromBody] CreateQuestionDto dto,
            [FromServices] IMediator mediator,
            [FromQuery] bool draft = false,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new CreateQuestionCommand(dto, draft);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateQuestionDto>>();

        #endregion POST

        #region DELETE

        group.MapDelete("/{id:guid}", async Task<Results<Ok<DeleteQuestionResponse>, ProblemHttpResult>> (
            Guid id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
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

        #endregion DELETE

        #region PUT

        group.MapPut("/", async Task<Results<Ok<UpdateQuestionResponse>, ProblemHttpResult>> (
            [FromBody] UpdateQuestionDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
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

        #endregion PUT
    }
}
