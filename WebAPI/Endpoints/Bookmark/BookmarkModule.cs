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
        var group = endpoints.MapGroup(EG.Bookmark)
                           .WithTags(nameof(BookmarkModule))
                           .WithOpenApi();

        group.MapGet("/", HandleGetBookmarks)
             .WithName("GetUserBookmarks")
             .WithSummary("Get user's bookmarked questions")
             .WithDescription("Retrieves a paginated list of questions that the authenticated user has bookmarked")
             .RequireAuthorization();

        group.MapPost("/{questionId:int}", HandleCreateBookmark)
             .WithName("CreateBookmark")
             .WithSummary("Bookmark a question")
             .WithDescription("Adds the specified question to the user's bookmarks for later reference")
             .RequireAuthorization();

        group.MapDelete("/{bookmarkId:int}", HandleDeleteBookmark)
             .WithName("DeleteBookmark")
             .WithSummary("Remove a bookmark")
             .WithDescription("Removes the specified bookmark from the user's bookmarked questions")
             .RequireAuthorization();
    }

    private static async Task<Results<Ok<PagedResponse<BookmarkResponse>>, ProblemHttpResult>> HandleGetBookmarks(
        string orderBy,
        int pageIndex,
        int pageSize,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new GetUserBookmarkQuery(orderBy, PageArgs.From(pageIndex, pageSize));
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> HandleCreateBookmark(
        int questionId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new CreateBookmarkCommand(questionId);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> HandleDeleteBookmark(
        int bookmarkId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new DeleteBookmarkCommand(bookmarkId);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }
}
