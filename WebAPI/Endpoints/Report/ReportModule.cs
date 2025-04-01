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
}
