﻿using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Response.AuthResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;
using E = WebAPI.Utilities.Constants.Endpoints;

namespace WebAPI.Endpoints.Auth;

public sealed class AuthModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Auth)
                           .WithTags(nameof(AuthModule))
                           .WithOpenApi();

        group.MapGet("/", () =>
        {
            return Results.Ok("Hello");
        })
            .WithName("HealthCheck")
            .WithSummary("API Health Check")
            .WithDescription("Simple endpoint to verify the auth service is running");

        group.MapPost("/refresh",
            async Task<Results<Ok<AuthRefreshResponse>, ProblemHttpResult>> (
            [FromBody] RefreshTokenRequestDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new RefreshTokenCommand(dto);
            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }
            return TypedResults.Ok(result.Value);
        })
            .WithName("RefreshToken")
            .WithSummary("Refresh authentication token")
            .WithDescription("Generates a new access token using a valid refresh token")
            .RequireAuthorization()
            .AllowAnonymous();

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
            .WithName("Register")
            .WithSummary("Register new user")
            .WithDescription("Creates a new user account with the provided credentials and returns authentication tokens")
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
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticates user credentials and returns access and refresh tokens")
            .AllowAnonymous()
            .AddEndpointFilter<FluentValidation<LoginDto>>();
    }
}
