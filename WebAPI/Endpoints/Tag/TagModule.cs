using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Requirement;
using WebAPI.Filters.Validation;
using WebAPI.Pagination;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Provider;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Tag;

public class TagModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Tag);

        group.MapGet("/{id:guid}",
            async Task<Results<Ok<TagDetailResponse>, ProblemHttpResult>> (
             Guid id,
             [FromServices] IMediator mediator,
             CancellationToken cancellationToken) =>
            {
                var command = new GetTagDetailQuery(id);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : ProblemResultExtensions.BadRequest(result.Message);
            })
            .RequireAuthorization();

        group.MapGet("/search/{keyword}",
            async Task<Results<Ok<PagedResponse<TagResponse>>, ProblemHttpResult>> (
             string keyword,
             [FromServices] IMediator mediator,
             CancellationToken cancellationToken) =>
            {
                var command = new SearchTagByKeywordQuery(keyword, new() { Page = 1, PageSize = 6 });
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : ProblemResultExtensions.BadRequest(result.Message);
            })
            .RequireAuthorization();

        group.MapGet("/all",
            async Task<Results<Ok<List<TagResponse>>, ProblemHttpResult>> (
             [FromServices] IMediator mediator,
             CancellationToken cancellationToken) =>
            {
                var command = new GetAllTagQuery();
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : ProblemResultExtensions.BadRequest(result.Message);
            })
            .RequireAuthorization();

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
