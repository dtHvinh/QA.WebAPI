using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Response;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.User;

public class UserModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.User);

        group.MapGet(
            "/",
            async Task<Results<Ok<UserResponse>, ProblemHttpResult>>
            ([FromServices] IMediator mediator,
            [FromQuery] bool? noCache,
            CancellationToken cancellationToken = default) =>
            {
                var query = new GetUserQuery(noCache ?? true);

                var result = await mediator.Send(query, cancellationToken);

                if (!result.IsSuccess)
                {
                    return ProblemResultExtensions.BadRequest(result.Message);
                }

                return TypedResults.Ok(result.Value);

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
