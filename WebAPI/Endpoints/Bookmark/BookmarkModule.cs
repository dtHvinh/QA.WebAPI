using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Pagination;
using WebAPI.Response;
using WebAPI.Response.BookmarkResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Bookmark;

public class BookmarkModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Bookmark).WithTags(nameof(BookmarkModule));


        group.MapGet("/", async Task<Results<Ok<PagedResponse<BookmarkResponse>>, ProblemHttpResult>> (
            string orderBy,
            int pageIndex,
            int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new GetUserBookmarkQuery(orderBy, PageArgs.From(pageIndex, pageSize));
            var result = await mediator.Send(cmd, cancellationToken);
            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
        .RequireAuthorization();

        group.MapPost("/{questionId:int}", async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new CreateBookmarkCommand(questionId);
            var result = await mediator.Send(cmd, cancellationToken);
            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
        .RequireAuthorization();

        group.MapDelete("/{bookmarkId:int}", async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            int bookmarkId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new DeleteBookmarkCommand(bookmarkId);
            var result = await mediator.Send(cmd, cancellationToken);
            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
        .RequireAuthorization();
    }
}
