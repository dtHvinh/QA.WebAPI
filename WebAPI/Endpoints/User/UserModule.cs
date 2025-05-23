using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Options;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.User;

public class UserModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.User)
            .WithTags(nameof(UserModule))
            .WithOpenApi();

        group.MapGet("/{userId}/is_role/{role}",
            async (int userId, string role, UserManager<ApplicationUser> roleManager, AuthenticationContext authContext) =>
        {
            var isRole = await roleManager.IsInRoleAsync(new() { Id = userId, }, role);
            return TypedResults.Ok(isRole);
        });

        group.MapGet("/{username?}", HandleGetUser)
            .WithName("GetUserProfile")
            .WithSummary("Get user profile")
            .WithDescription("Retrieves user profile information. If username is not provided, returns the current user's profile")
            .RequireAuthorization();

        group.MapGet("/is_allow_to/{action}", HandleCheckPermission)
            .WithName("CheckUserPermission")
            .WithSummary("Check user permissions")
            .WithDescription("Checks if the user has enough reputation to perform specific actions (createTag, comment, upvote, downvote)")
            .RequireAuthorization();

        group.MapPut("/", HandleUpdateUser)
            .WithName("UpdateUserProfile")
            .WithSummary("Update user profile")
            .WithDescription("Updates the current user's profile information")
            .RequireAuthorization();

        group.MapPut("/profile-picture", HandleUpdateUserPfp)
            .WithName("UpdateUserProfilePicture")
            .WithSummary("Update user profile picture")
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<UserResponse>, ProblemHttpResult>> HandleGetUser(
        string? username,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserQuery(username);
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> HandleUpdateUserPfp(
        [FromForm] UpdatePfpDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new UpdateProfileImageCommand(dto.ProfilePicture);
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<PermissionResponse>, ProblemHttpResult>> HandleCheckPermission(
        string action,
        IOptions<ApplicationProperties> options,
        IUserRepository userRepository,
        AuthenticationContext authContext,
        CancellationToken cancellationToken = default)
    {
        var req = options.Value.ActionRepRequirement;
        var reqRep = action switch
        {
            "createTag" => req.CreateTags,
            "comment" => req.Comment,
            "upvote" => req.Upvote,
            "downvote" => req.Downvote,
            "editTag" => req.EditTagWiki,
            _ => 9999
        };
        var userRep = await userRepository.GetReputation(authContext.UserId, cancellationToken);
        return TypedResults.Ok(new PermissionResponse(userRep >= reqRep, reqRep));
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> HandleUpdateUser(
        [FromBody] UpdateUserDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }
}
