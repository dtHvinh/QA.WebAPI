using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Queries;
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

        group.MapGet("/users", GetUsers)
           .RequireAuthorization()
           .WithName("GetUsers")
           .WithSummary("Get all users")
           .WithDescription("Retrieves all users in the system")
           .WithOpenApi();
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
}
