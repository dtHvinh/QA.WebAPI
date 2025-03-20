using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Pagination;
using WebAPI.Response;
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

        group.MapPost("/join/{communityId}", JoinCommunity)
            .WithName("JoinCommunity")
            .WithSummary("Join community")
            .DisableAntiforgery()
            .RequireAuthorization();

        group.MapPost("/room", CreateCommunityChatRoom)
            .WithName("CreateCommunityChatRoom")
            .WithSummary("Create new community chat room")
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCommunityChatRoomDto>>();

        group.MapGet("/room/{communityId}", GetCommunityRoom)
            .WithName("GetCommunityChatRoom")
            .RequireAuthorization();

        group.MapGet("/", GetCommunities)
            .RequireAuthorization();

        group.MapGet("/popular", GetPopularCommunities)
            .RequireAuthorization();

        group.MapGet("/detail/{name}", GetCommunityDetail)
            .RequireAuthorization();

        group.MapGet("/joined", GetJoinedCommunities)
            .RequireAuthorization();

        group.MapGet("/search/{searchTerm}", SearchCommunities)
            .RequireAuthorization();

        group.MapDelete("/{communityId}/room/{roomId}", DeleteCommunityChatRoom)
            .WithName("DeleteCommunityChatRoom")
            .WithSummary("Delete community chat room")
            .DisableAntiforgery()
            .RequireAuthorization();

        group.MapDelete("/{communityId}/leave", LeaveCommunity)
            .WithName("LeaveCommunity")
            .WithSummary("Leave community")
            .DisableAntiforgery()
            .RequireAuthorization();

        group.MapPut("/room", UpdateChatRoom)
            .WithName("UpdateChatRoom")
            .WithSummary("Update a chat room")
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateChatRoomDto>>();
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


    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> LeaveCommunity(
        int communityId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new LeaveCommunityCommand(communityId);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> UpdateChatRoom(
        [FromBody] UpdateChatRoomDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateChatRoomCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> JoinCommunity(
        int communityId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new JoinCommunityCommand(communityId);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> DeleteCommunityChatRoom(
        int communityId,
        int roomId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteChatRoomCommand(communityId, roomId);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<List<ChatRoomResponse>>, ProblemHttpResult>> GetCommunityRoom(
        int communityId,
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCommunityChatRoomQuery(communityId, pageArgs);
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<List<GetCommunityResponse>>, ProblemHttpResult>> SearchCommunities(
        string searchTerm,
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new SearchCommunityQuery(searchTerm, pageArgs);
        var result = await mediator.Send(query, cancellationToken);
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
