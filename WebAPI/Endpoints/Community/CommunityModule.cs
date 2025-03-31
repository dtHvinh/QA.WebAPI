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

        group.MapPost("/room/chat", SendChatRoomMessage)
            .WithName("SendChatRoomMessage")
            .WithSummary("Send chat room message")
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<ChatRequestDto>>();

        group.MapPost("/{communityId:int}/mod/grant/{userId:int}", GrantModRole)
            .WithName("GrantModRole")
            .WithSummary("Grant moderator role")
            .DisableAntiforgery()
            .RequireAuthorization();

        group.MapPost("/{communityId:int}/mod/revoke/{userId:int}", RevokeModRole)
            .WithName("RevokeModRole")
            .WithSummary("Revoke moderator role")
            .DisableAntiforgery()
            .RequireAuthorization();

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

        group.MapGet("/room/chat/{roomId}", GetChatRoomMessage)
            .WithName("GetChatRoomMessage")
            .RequireAuthorization();

        group.MapGet("/room/{communityId}", GetCommunityRoom)
            .WithName("GetCommunityChatRoom")
            .RequireAuthorization();

        group.MapGet("/", GetCommunities)
            .RequireAuthorization();

        group.MapGet("/popular", GetPopularCommunities)
            .RequireAuthorization();

        group.MapGet("/{communityId}/members", GetCommunityMembers)
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
            .RequireAuthorization();

        group.MapDelete("/{communityId}/", DeleteCommunity)
            .WithName("DeleteCommunity")
            .WithSummary("Delete community")
            .RequireAuthorization();

        group.MapDelete("/{communityId}/leave", LeaveCommunity)
            .WithName("LeaveCommunity")
            .WithSummary("Leave community")
            .DisableAntiforgery()
            .RequireAuthorization();

        group.MapPut("/", UpdateCommunity)
            .WithName("UpdateCommunity")
            .WithSummary("Update communitiy")
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateCommunityDto>>();

        group.MapPut("/room", UpdateChatRoom)
            .WithName("UpdateChatRoom")
            .WithSummary("Update a chat room")
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateChatRoomDto>>();

        group.MapDelete("/{communityId}/member/{memberId}", DeleteCommunityMember)
            .WithName("DeleteCommunityMember")
            .WithSummary("Delete community member")
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

    private static async Task<Results<Ok<PagedResponse<ChatMessageResponse>>, ProblemHttpResult>> GetChatRoomMessage(
        int roomId,
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetChatRoomMessageQuery(roomId, pageArgs);
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<ChatMessageResponse>, ProblemHttpResult>> SendChatRoomMessage(
        [FromForm] ChatRequestDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ChatCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> DeleteCommunityMember(
        int communityId,
        int memberId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCommunityMemberCommand(communityId, memberId);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> RevokeModRole(
        int communityId,
        int userId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CommunityModRoleCommand(userId, communityId, false);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> GrantModRole(
        int communityId,
        int userId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CommunityModRoleCommand(userId, communityId, true);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> UpdateCommunity(
        [FromForm] UpdateCommunityDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCommunityCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> DeleteCommunity(
        int communityId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCommunityCommand(communityId);
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

    private static async Task<Results<Ok<PagedResponse<CommunityMemberResponse>>, ProblemHttpResult>> GetCommunityMembers(
        int communityId,
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new GetCommunityMemberQuery(communityId, pageArgs);
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
