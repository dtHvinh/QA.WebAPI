using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class ResolveReportHandler(
    IReportRepository reportRepository,
    Serilog.ILogger logger,
    AuthenticationContext authenticationContext)
    : ICommandHandler<ResolveReportCommand, GenericResult<TextResponse>>
{
    private readonly IReportRepository _reportRepository = reportRepository;
    private readonly Serilog.ILogger _logger = logger;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(ResolveReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.FindFirstAsync(
            x => x.Id == request.ReportId,
            cancellationToken: cancellationToken
        );

        if (report is null)
        {
            return GenericResult<TextResponse>.Failure("Report not found");
        }

        report.Status = ReportStatus.Resolved;

        _reportRepository.Update(report);

        var result = await _reportRepository.SaveChangesAsync(cancellationToken);

        _logger.ModeratorNoEnityOwnerAction(result.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error,
            _authenticationContext.UserId,
            LogModeratorOp.Resolved,
            report);

        return result.IsSuccess
            ? GenericResult<TextResponse>.Success(new TextResponse("Report resolved"))
            : GenericResult<TextResponse>.Failure("Failed to resolve report");
    }
}
