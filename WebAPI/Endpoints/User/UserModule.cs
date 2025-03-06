using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
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
        var group = endpoints.MapGroup(EG.User);

        group.MapGet(
            "/{username?}",
            async Task<Results<Ok<UserResponse>, ProblemHttpResult>> (
                [FromServices] IMediator mediator,
                string? username,
                CancellationToken cancellationToken = default) =>
            {
                var query = new GetUserQuery(username);
                var result = await mediator.Send(query, cancellationToken);

                if (!result.IsSuccess)
                {
                    return ProblemResultExtensions.BadRequest(result.Message);
                }

                return TypedResults.Ok(result.Value);
            })
            .RequireAuthorization();

        group.MapGet("/is_allow_to/{action}",
            async Task<Results<Ok<PermissionResponse>, ProblemHttpResult>> (
                string action,
                IOptions<ApplicationProperties> options,
                IUserRepository userRepository,
                AuthenticationContext authContext,
                CancellationToken cancellationToken = default) =>
            {
                var req = options.Value.ActionRepRequirement;

                var reqRep = action switch
                {
                    "createTag" => req.CreateTags,
                    "comment" => req.Comment,
                    "upvote" => req.Upvote,
                    "downvote" => req.Downvote,
                    _ => 9999
                };

                var userRep = await userRepository.GetReputation(authContext.UserId, cancellationToken);

                return TypedResults.Ok(new PermissionResponse(userRep >= reqRep, reqRep));
            })
            .RequireAuthorization();

        group.MapPut(
            "/",
            async Task<Results<Ok<GenericResponse>, ProblemHttpResult>>
            ([FromBody] UpdateUserDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
            {
                var command = new UpdateUserCommand(dto);

                var result = await mediator.Send(command, cancellationToken);

                if (!result.IsSuccess)
                {
                    return ProblemResultExtensions.BadRequest(result.Message);
                }

                return TypedResults.Ok(result.Value);

            })
            .RequireAuthorization();
    }
}
