using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Response;
using static WebAPI.Utilities.Constants;
using E = WebAPI.Utilities.Constants.Endpoints;

namespace WebAPI.Endpoints.Auth;

public sealed class AuthModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Auth);

        group.MapGet("/", () =>
        {
            return Results.Ok("Hello");
        });

        MapLogin(group);
        MapRegister(group);
    }

    private static void MapRegister(RouteGroupBuilder group)
    {
        group.MapPost(E.Register,
                      async Task<Results<Ok<AuthResponse>, ProblemHttpResult>> (
                                        [FromBody] RegisterDto dto,
                                        [FromServices] IMediator mediator,
                                        CancellationToken cancellationToken) =>
        {
            var command = new CreateUserCommand(dto);

            var result = await mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .AllowAnonymous()
            .AddEndpointFilter<FluentValidation<RegisterDto>>();
    }

    private static void MapLogin(RouteGroupBuilder group)
    {
        group.MapPost(E.Login,
            async Task<Results<Ok<AuthResponse>, ProblemHttpResult>> (
                [FromBody] LoginDto dto,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var query = new LoginQuery(dto);

            var result = await mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .AllowAnonymous()
            .AddEndpointFilter<FluentValidation<LoginDto>>();
    }
}
