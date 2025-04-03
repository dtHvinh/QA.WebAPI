using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Endpoints.Community;
using WebAPI.Filters.Requirement.RoleReq;
using WebAPI.Pagination;
using WebAPI.Response;
using WebAPI.Response.QuestionResponses;
using WebAPI.Response.Reports;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Moderator;

public class ModeratorModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Mod)
              .WithTags(nameof(CommunityModule))
              .WithOpenApi();

        group.MapGet("/questions/all", ModGetQuestion)
            .WithName("ModGetQuestions")
            .WithSummary("Mod Get all questions")
            .AddEndpointFilter<RequireModerator>()
            .RequireAuthorization();

        group.MapGet("/question/{questionId:int}/", ModGetQuestionById)
            .WithName("ModGetQuestionById")
            .AddEndpointFilter<RequireModerator>()
            .RequireAuthorization();

        group.MapGet("/reports/{type?}", GetAllReport)
            .WithName("GetReport")
            .AddEndpointFilter<RequireModerator>()
            .RequireAuthorization();

        group.MapPut("/question/{questionId:int}/restore", ModRestoreQuestion)
            .WithName("ModCloseQuestion")
            .AddEndpointFilter<RequireModerator>()
            .RequireAuthorization();

        group.MapDelete("/question/{questionId:int}", ModDeleteQuestion)
            .WithName("ModDeleteQuestion")
            .AddEndpointFilter<RequireModerator>()
            .RequireAuthorization();
    }

    private static async
    Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> ModGetQuestion(
        [AsParameters] PageArgs pageArgs,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new ModGetQuestionQuery(pageArgs);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async
    Task<Results<Ok<GetQuestionResponse>, ProblemHttpResult>> ModGetQuestionById(
        int questionId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new ModGetQuestionByIdQuery(questionId);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async
    Task<Results<Ok<TextResponse>, ProblemHttpResult>> ModRestoreQuestion(
        int questionId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new ModRestoreQuestionCommand(questionId);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async
    Task<Results<Ok<TextResponse>, ProblemHttpResult>> ModDeleteQuestion(
        int questionId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new ModDeleteQuestionCommand(questionId);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }


    private static async Task<Results<Ok<PagedResponse<GetReportResponse>>, ProblemHttpResult>> GetAllReport(
        string? type,
        [AsParameters] PageArgs pageArgs,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllReportQuery(pageArgs, type);
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }
}
