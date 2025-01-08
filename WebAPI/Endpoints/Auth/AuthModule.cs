using Microsoft.AspNetCore.Mvc;
using WebAPI.Dto;
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
        group.MapPost(E.Register, ([FromBody] RegisterDto dto, [FromServices] ILogger logger) =>
        {
            logger.LogInformation("Hihi");
            return Results.Ok(dto);
        }).AllowAnonymous();
    }

    private static void MapLogin(RouteGroupBuilder group)
    {
        group.MapPost(E.Login, ([FromBody] LoginDto dto, [FromServices] ILogger logger) =>
        {
            return Results.Ok(dto);
        }).AllowAnonymous();
    }
}
