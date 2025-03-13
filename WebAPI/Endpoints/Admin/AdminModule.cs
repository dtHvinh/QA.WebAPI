using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Pagination;
using WebAPI.Response;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Admin;

public class AdminModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Admin)
            .WithTags(nameof(AdminModule))
            .WithOpenApi();

        group.MapGet("/analytic/{what}/{period}", GetAnalytic)
           .RequireAuthorization()
           .WithName("GetAnalytics")
           .WithSummary("Get system analytics data")
           .WithDescription("Retrieves various system analytics based on the specified category")
           .WithOpenApi(operation =>
           {
               operation.Parameters[0].Description = "Analytics category (users, questions, answers, etc.)";
               return operation;
           });

        group.MapGet("/ban-info/{userId:int}", GetUserBanInfo)
           .RequireAuthorization()
           .WithName("GetUserBanInfo")
           .WithSummary("Get user ban info")
           .WithOpenApi();

        group.MapGet("/user-role/{userId:int}", GetUserRole)
           .RequireAuthorization()
           .WithName("GetUserRole")
           .WithSummary("Get user roles")
           .WithOpenApi();

        group.MapGet("/roles", GetAppRoles)
           .RequireAuthorization()
           .WithName("GetAppRoles")
           .WithSummary("Get application roles")
           .WithOpenApi();

        group.MapPost("/roles/{userId:int}/{role}", AddToRole)
           .RequireAuthorization()
           .WithName("AddToRole")
           .WithSummary("Add user to role")
           .WithOpenApi();

        group.MapGet("/users", GetUsers)
           .RequireAuthorization()
           .WithName("GetUsers")
           .WithSummary("Get all users")
           .WithDescription("Retrieves all users in the system")
           .WithOpenApi();

        group.MapPost("/ban/{userId}", BanUser)
           .RequireAuthorization()
           .AddEndpointFilter<FluentValidation<BanUserDto>>()
           .WithName("BanUser")
           .WithSummary("Ban a user")
           .WithDescription("Ban a user from the system");


        group.MapDelete("/unban/{userId}", UnbanUser)
           .RequireAuthorization()
           .WithName("UnbanUser")
           .WithSummary("Unban a user")
           .WithDescription("Revoke a ban of an user");
    }

    private static async Task<Results<Ok<List<RoleResponse>>, ProblemHttpResult>> GetUserRole(
        int userId,

      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default)
    {
        var query = new GetUserRoleQuery(userId);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }


    private static async Task<Results<Ok<List<string>>, ProblemHttpResult>> GetAppRoles(
    [FromServices] RoleManager<IdentityRole<int>> rm,
      CancellationToken cancellationToken = default)
    {
        var roles = await rm.Roles.Select(e => e.Name!).ToListAsync(cancellationToken);

        return TypedResults.Ok(roles);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> AddToRole(
        int userId,
        string role,

      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default)
    {
        var command = new AddUserToRoleCommand(userId, role);

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<GrownAnalyticResponse>, ProblemHttpResult>> GetAnalytic(
      string what,
      string period,

      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default)
    {
        var query = new AdminAnalyticQuery(what, period);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<BanInfoResponse>, ProblemHttpResult>> GetUserBanInfo(
      int userId,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default)
    {
        var query = new GetBanUserInfoQuery(userId);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<PagedResponse<GetUserResponse>>, ProblemHttpResult>> GetUsers(
      [AsParameters] PageArgs pageArgs,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default)
    {
        var query = new AdminGetUserQuery(pageArgs);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> BanUser(
      [FromBody] BanUserDto banUserDto,
      int userId,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default)
    {
        var query = new BanCommand(userId, banUserDto);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> UnbanUser(
      int userId,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default)
    {
        var query = new UnbanCommand(userId);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }
}
