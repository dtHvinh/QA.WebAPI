using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class RejectReportHandler(
    IReportRepository reportRepository,
    Serilog.ILogger logger,
    AuthenticationContext authenticationContext)
    : ICommandHandler<RejectReportCommand, GenericResult<TextResponse>>
{
    private readonly IReportRepository reportRepository = reportRepository;
    private readonly Serilog.ILogger _logger = logger;
    private readonly AuthenticationContext authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(RejectReportCommand request, CancellationToken cancellationToken)
    {
        var report = await reportRepository.FindFirstAsync(
            x => x.Id == request.ReportId,
            cancellationToken: cancellationToken
        );
        if (report is null)
        {
            return GenericResult<TextResponse>.Failure("Report not found");
        }
        report.Status = ReportStatus.Rejected;

        reportRepository.Update(report);

        var result = await reportRepository.SaveChangesAsync(cancellationToken);

        _logger.ModeratorNoEnityOwnerAction(result.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error,
            authenticationContext.UserId,
            LogModeratorOp.Rejected,
            report);

        return result.IsSuccess
            ? GenericResult<TextResponse>.Success(new TextResponse("Report rejected"))
            : GenericResult<TextResponse>.Failure("Failed to reject report");
    }
}
