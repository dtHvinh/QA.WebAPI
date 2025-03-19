using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Pagination;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Community;

public class CommunityModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Community)
            .WithTags(nameof(CommunityModule))
            .WithOpenApi();

        group.MapPost("/", CreateCommunity)
            .WithName("CreateCommunity")
            .WithSummary("Create new community")
            .WithDescription("Creates a new community with optional icon image")
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCommunityDto>>();

        group.MapPost("/room", CreateCommunityChatRoom)
            .WithName("CreateCommunityChatRoom")
            .WithSummary("Create new community chat room")
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCommunityChatRoomDto>>();

        group.MapGet("/", GetCommunities)
            .RequireAuthorization();

        group.MapGet("/popular", GetPopularCommunities)
            .RequireAuthorization();

        group.MapGet("/detail/{name}", GetCommunityDetail)
            .RequireAuthorization();

        group.MapGet("/joined", GetJoinedCommunities)
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<CreateCommunityResponse>, ProblemHttpResult>> CreateCommunity(
        [FromForm] CreateCommunityDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateCommunityCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<CreateChatRoomResponse>, ProblemHttpResult>> CreateCommunityChatRoom(
        [FromBody] CreateCommunityChatRoomDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateChatRoomCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<GetCommunityDetailResponse>, ProblemHttpResult>> GetCommunityDetail(
    string name,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken)
    {
        var command = new GetCommunityDetailQuery(name);

        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<List<GetCommunityResponse>>, ProblemHttpResult>> GetCommunities(
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCommunityQuery(pageArgs);

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<List<GetCommunityResponse>>, ProblemHttpResult>> GetPopularCommunities(
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPopularCommunityQuery(pageArgs);

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<List<GetCommunityResponse>>, ProblemHttpResult>> GetJoinedCommunities(
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetJoinedCommunitiesQuery(pageArgs);

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }
}
