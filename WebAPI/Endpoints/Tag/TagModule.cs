using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Requirement;
using WebAPI.Filters.Validation;
using WebAPI.Pagination;
using WebAPI.Response;
using WebAPI.Response.QuestionResponses;
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

        group.MapGet("/{id:int}",
                async Task<Results<Ok<TagWithQuestionResponse>, ProblemHttpResult>> (
                    int id,
                    string orderBy,
                    int pageIndex,
                    int pageSize,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetTagWithQuestionQuery(id, orderBy, PageArgs.From(pageIndex, pageSize));
                    var result = await mediator.Send(query, cancellationToken);

                    return result.IsSuccess
                        ? TypedResults.Ok(result.Value)
                        : ProblemResultExtensions.BadRequest(result.Message);
                })
            .RequireAuthorization();

        group.MapGet("/wiki/{id:int}",
                async Task<Results<Ok<TagWithWikiBodyResponse>, ProblemHttpResult>> (
                    int id,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetTagDetailQuery(id);
                    var result = await mediator.Send(query, cancellationToken);

                    return result.IsSuccess
                        ? TypedResults.Ok(result.Value)
                        : ProblemResultExtensions.BadRequest(result.Message);
                })
            .RequireAuthorization();

        group.MapGet("/{tagId:int}/questions/",
                async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> (
                    int tagId,
                    string orderBy,
                    int pageIndex,
                    int pageSize,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetTagQuestionQuery(tagId, orderBy, PageArgs.From(pageIndex, pageSize));
                    var result = await mediator.Send(query, cancellationToken);

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
                    var query = new SearchTagByKeywordQuery(keyword, new() { Page = 1, PageSize = 8 });
                    var result = await mediator.Send(query, cancellationToken);

                    return result.IsSuccess
                        ? TypedResults.Ok(result.Value)
                        : ProblemResultExtensions.BadRequest(result.Message);
                })
            .RequireAuthorization();

        group.MapGet("/{orderBy}",
                async Task<Results<Ok<PagedResponse<TagResponse>>, ProblemHttpResult>> (
                    string orderBy,
                    int skip,
                    int take,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetTagQuery(orderBy, skip, take);
                    var result = await mediator.Send(query, cancellationToken);

                    return result.IsSuccess
                        ? TypedResults.Ok(result.Value)
                        : ProblemResultExtensions.BadRequest(result.Message);
                })
            .RequireAuthorization();

        group.MapPost("/",
                async Task<Results<Ok<GenericResponse>, ProblemHttpResult>>
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
            .AddEndpointFilter<FluentValidation<CreateTagDto>>();


        group.MapPut("/",
                async Task<Results<Ok<GenericResponse>, ProblemHttpResult>>
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
            .AddEndpointFilter<FluentValidation<UpdateTagDto>>();

        group.MapDelete("/{id:int}",
                async Task<Results<Ok<DeleteTagResponse>, ProblemHttpResult>>
                (int id,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeleteTagCommand(id);
                    var result = await mediator.Send(command, cancellationToken);

                    return result.IsSuccess
                        ? TypedResults.Ok(result.Value)
                        : ProblemResultExtensions.BadRequest(result.Message);
                })
            .RequireAuthorization(PolicyProvider.RequireAdminRole);
    }
}