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
        var group = endpoints.MapGroup(EG.Report);

        group.MapPost("/question",
            async Task<Results<Ok<CreateReportResponse>, ProblemHttpResult>> (
                [FromBody] CreateReportDto dto,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var command = new CreateQuestionReportCommand(dto);

            var result = await mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : ProblemResultExtensions.BadRequest(result.Message);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateReportDto>>();

        group.MapPost("/answer",
            async Task<Results<Ok<CreateReportResponse>, ProblemHttpResult>> (
                [FromBody] CreateReportDto dto,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateAnswerReportCommand(dto);

                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                            ? TypedResults.Ok(result.Value)
                            : ProblemResultExtensions.BadRequest(result.Message);
            })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateReportDto>>();
    }
}
