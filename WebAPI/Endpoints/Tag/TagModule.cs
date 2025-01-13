using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Filters.Requirement;
using WebAPI.Filters.Validation;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Response;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Tag;

public class TagModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Tag);

        group.MapPost("/",
            async Task<Results<Ok<CreateTagResponse>, ProblemHttpResult>>
            ([FromBody] CreateTagDto dto,
             [FromServices] IMediator mediator,
             CancellationToken cancellationToken) =>
        {
            var command = new CreateTagCommand(dto);
            var result = await mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : ProblemResultExtensions.BadRequest(result.Message);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateTagDto>>()
            .AddEndpointFilter<CreateTagReputationRequirementFilter>();


        group.MapPut("/",
            async Task<Results<Ok<UpdateTagResponse>, ProblemHttpResult>>
            ([FromBody] UpdateTagDto dto,
             [FromServices] IMediator mediator,
             CancellationToken cancellationToken) =>
        {
            var command = new UpdateTagCommand(dto);
            var result = await mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : ProblemResultExtensions.BadRequest(result.Message);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateTagDto>>()
            .AddEndpointFilter<UpdateTagReputationRequirementFilter>();

        group.MapDelete("/{id:guid}",
            async Task<Results<Ok<DeleteTagResponse>, ProblemHttpResult>>
            (Guid id,
             [FromServices] IMediator mediator,
             CancellationToken cancellationToken) =>
        {
            var command = new DeleteTagCommand(id);
            var result = await mediator.Send(command, cancellationToken);

            return result.IsSuccess
                        ? TypedResults.Ok(result.Value)
                        : ProblemResultExtensions.BadRequest(result.Message);
        })
            .RequireAuthorization(PolicyProvider.RequireAdminRole)
            .AddEndpointFilter<DeleteTagReputationRequirementFilter>();

    }
}
