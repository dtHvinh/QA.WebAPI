using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Utilities.Contract;
using E = WebAPI.Utilities.Constants.Endpoints;
using EG = WebAPI.Utilities.Constants.EndpointGroup;

namespace WebAPI.Endpoints.Auth;

public class AuthModule : IModule
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
                      async Task<Results<Ok<AuthResponseDto>, BadRequest<string>>> (
                                        [FromBody] RegisterDto dto,
                                        [FromServices] IMediator mediator,
                                        CancellationToken cancellationToken) =>
        {
            var command = new CreateUserCommand(dto);

            var result = await mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return TypedResults.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .AddEndpointFilter<FluentValidationFilter<RegisterDto>>();
    }

    private static void MapLogin(RouteGroupBuilder group)
    {
        group.MapPost(E.Login, ([FromBody] LoginDto dto, [FromServices] ILogger logger) =>
        {
            return Results.Ok(dto);
        }).AllowAnonymous();
    }
}
