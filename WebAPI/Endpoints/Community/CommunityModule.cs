using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Pagination;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Community;

public class CommunityModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Community);

        group.MapPost("/", CreateCommunity)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCommunityDto>>();

        group.MapGet("/", GetCommunities)
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<CreateCommunityResponse>, ProblemHttpResult>> CreateCommunity(
        [FromBody] CreateCommunityDto dto,
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
}
