using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Collection;

public class CollectionModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Collection);

        group.MapGet("/with_question/{questionId}", HandleGetCollectionAndAddStatus)
            .RequireAuthorization();

        group.MapGet("/{id}", HandleGetCollectionDetail)
            .RequireAuthorization();

        group.MapGet("/my-collections", HandleGetUserCollection)
            .RequireAuthorization();

        group.MapPost("/", HandleCreateCollection)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCollectionDto>>();

        group.MapPut("/", HandleUpdateCollection)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateCollectionDto>>();

        group.MapDelete("/{id}", HandleDeleteCollection)
            .RequireAuthorization();
    }

    public static
        async Task<Results<Ok<List<GetCollectionWithAddStatusResponse>>, ProblemHttpResult>> HandleGetCollectionAndAddStatus(
        int questionId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserCollectionAndAddStatusQuery(questionId);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }


    public static
        async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleCreateCollection(
        [FromBody] CreateCollectionDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateCollectionCommand(dto);

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    public static
        async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleUpdateCollection(
        [FromBody] UpdateCollectionDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCollectionCommand(dto);

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    public static
        async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleDeleteCollection(
        int id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteQuestionCollectionCommand(id);

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    public static
        async Task<Results<Ok<GetCollectionDetailResponse>, ProblemHttpResult>> HandleGetCollectionDetail(
        int id,
        [FromQuery] int pageIndex,
        [FromQuery] int pageSize,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new GetCollectionDetailQuery(id, PageArgs.From(pageIndex, pageSize));

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    public static
        async Task<Results<Ok<PagedResponse<GetCollectionResponse>>, ProblemHttpResult>> HandleGetUserCollection(
        string orderBy,
        int pageIndex,
        int pageSize,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserCollectionQuery(
            PageArgs.From(pageIndex, pageSize), Enum.Parse<CollectionSortOrder>(orderBy, true));

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }
}
