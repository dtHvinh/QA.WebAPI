using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Response;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Report;

public class ReportModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Report)
            .WithTags(nameof(ReportModule))
            .WithOpenApi();

        group.MapPost("/question", CreateReport)
            .WithName("CreateReport")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateReportDto>>();

        group.MapPut("/{reportId}/reject", RejectReport)
            .WithName("RejectReport")
            .RequireAuthorization();

        group.MapPut("/{reportId}/resolve", ResolveReport)
            .WithName("ResolveReport")
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> CreateReport(
        [FromBody] CreateReportDto dto,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateReportCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> RejectReport(
        int reportId,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        var command = new RejectReportCommand(reportId);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<TextResponse>, ProblemHttpResult>> ResolveReport(
        int reportId,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        var command = new ResolveReportCommand(reportId);
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }
}
