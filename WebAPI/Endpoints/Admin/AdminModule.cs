using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Queries;
using WebAPI.Response;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Admin;

public class AdminModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Admin).WithTags(nameof(AdminModule));

        group.MapGet("/analytic/{what}",
           static async Task<Results<Ok<GrownAnalyticResponse>, ProblemHttpResult>> (
           string what,
           [FromServices] IMediator mediator,
           CancellationToken cancellationToken = default) =>
           {
               var query = new AdminAnalyticQuery(what);

               var result = await mediator.Send(query, cancellationToken);

               if (!result.IsSuccess)
               {
                   return ProblemResultExtensions.BadRequest(result.Message);
               }

               return TypedResults.Ok(result.Value);
           })
           .RequireAuthorization()
           .WithName("GetAnalytics")
           .WithSummary("Get system analytics data")
           .WithDescription("Retrieves various system analytics based on the specified category")
           .WithOpenApi(operation =>
           {
               operation.Parameters[0].Description = "Analytics category (users, questions, answers, etc.)";
               return operation;
           });
    }
}
