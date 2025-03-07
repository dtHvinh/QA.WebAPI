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
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Collection;

public class CollectionModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Collection)
            .WithTags(nameof(CollectionModule))
            .WithOpenApi();

        group.MapGet("/", HandleGetCollection)
            .WithName("GetCollections")
            .WithSummary("Get all collections")
            .WithDescription("Retrieves a paginated list of all public collections")
            .RequireAuthorization();

        group.MapGet("/search/{searchTerm}", HandleSearchCollection)
            .WithName("SearchCollections")
            .WithSummary("Search collections")
            .WithDescription("Search for collections by name or description")
            .RequireAuthorization();

        group.MapGet("/with_question/{questionId:int}", HandleGetCollectionAndAddStatus)
            .WithName("GetCollectionsWithQuestionStatus")
            .WithSummary("Get collections with question status")
            .WithDescription("Get collections with information about whether a specific question is included in each collection")
            .RequireAuthorization();

        group.MapGet("/{id}", HandleGetCollectionDetail)
            .WithName("GetCollectionDetail")
            .WithSummary("Get collection details")
            .WithDescription("Retrieves detailed information about a specific collection including its questions")
            .RequireAuthorization();

        group.MapGet("/{collectionId:int}/search/{searchTerm}", HandleSearchQuestionInCollection)
            .WithName("SearchQuestionsInCollection")
            .WithSummary("Search questions in collection")
            .WithDescription("Search for questions within a specific collection")
            .RequireAuthorization();

        group.MapGet("/my-collections", HandleGetUserCollection)
            .WithName("GetUserCollections")
            .WithSummary("Get user's collections")
            .WithDescription("Retrieves all collections created by the authenticated user")
            .RequireAuthorization();

        group.MapPost("/", HandleCreateCollection)
            .WithName("CreateCollection")
            .WithSummary("Create new collection")
            .WithDescription("Creates a new question collection for the authenticated user")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCollectionDto>>();

        group.MapPost("/{collectionId:int}/like", HandleLikeCollection)
            .WithName("LikeCollection")
            .WithSummary("Like a collection")
            .WithDescription("Adds the collection to user's liked collections")
            .RequireAuthorization();

        group.MapDelete("/{collectionId:int}/unlike", HandleUnlikeCollection)
            .WithName("UnlikeCollection")
            .WithSummary("Unlike a collection")
            .WithDescription("Removes the collection from user's liked collections")
            .RequireAuthorization();

        group.MapPost("/{collectionId:int}/{action:regex(^(add|delete)$)}/{questionId:int}",
                HandleQuestionCollectionOp)
            .WithName("ManageCollectionQuestions")
            .WithSummary("Manage collection questions")
            .WithDescription("Add or remove questions from a collection")
            .RequireAuthorization();

        group.MapPut("/", HandleUpdateCollection)
            .WithName("UpdateCollection")
            .WithSummary("Update collection")
            .WithDescription("Update an existing collection's details")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateCollectionDto>>();

        group.MapDelete("/{id}", HandleDeleteCollection)
            .WithName("DeleteCollection")
            .WithSummary("Delete collection")
            .WithDescription("Permanently removes a collection and its associations")
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleLikeCollection(
        int collectionId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateCollectionLikeCommand(collectionId);

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleUnlikeCollection(
        int collectionId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCollectionLikeCommand(collectionId);

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<PagedResponse<GetCollectionResponse>>, ProblemHttpResult>> HandleGetCollection(
        [FromQuery] string orderBy,
        [FromQuery] int pageIndex,
        [FromQuery] int pageSize,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCollectionsQuery(
            PageArgs.From(pageIndex, pageSize), Enum.Parse<CollectionSortOrder>(orderBy, true));

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<PagedResponse<GetCollectionResponse>>, ProblemHttpResult>>
        HandleSearchCollection(
            string searchTerm,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
    {
        var query = new SearchCollectionQuery(searchTerm, PageArgs.From(pageIndex, pageSize));

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleQuestionCollectionOp(
        int questionId,
        int collectionId,
        string action,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new CollectionQuestionOperationCommand(questionId, collectionId, Enum.Parse<Operations>(action, true));

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<List<GetQuestionResponse>>, ProblemHttpResult>>
        HandleSearchQuestionInCollection(
            int collectionId,
            string searchTerm,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
    {
        var query = new SearchQuestionInCollectionQuery(collectionId, searchTerm);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async
        Task<Results<Ok<PagedResponse<GetCollectionWithAddStatusResponse>>, ProblemHttpResult>>
        HandleGetCollectionAndAddStatus(
            int questionId,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
    {
        var query = new GetUserCollectionAndAddStatusQuery(questionId, PageArgs.From(pageIndex, pageSize));

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }


    private static async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleCreateCollection(
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

    private static async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleUpdateCollection(
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

    private static async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> HandleDeleteCollection(
        int id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCollectionCommand(id);

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<GetCollectionDetailResponse>, ProblemHttpResult>> HandleGetCollectionDetail(
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

    private static async Task<Results<Ok<PagedResponse<GetCollectionResponse>>, ProblemHttpResult>>
        HandleGetUserCollection(
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