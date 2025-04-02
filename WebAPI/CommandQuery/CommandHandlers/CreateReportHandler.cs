using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateReportHandler(
    IReportRepository reportRepository,
    Serilog.ILogger logger,
    AuthenticationContext authenticationContext)
    : ICommandHandler<CreateReportCommand, GenericResult<TextResponse>>
{
    private readonly IReportRepository _reportRepository = reportRepository;
    private readonly Serilog.ILogger _logger = logger;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Description = request.Dto.Description,
            CreatedAt = DateTime.Now,
            Status = "Pending",
            TargetId = request.Dto.TargetId,
            Type = request.Dto.TargetType,
        };

        _reportRepository.CreateReport(report);

        var res = await _reportRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId, LogOp.Report, report);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success("Report created successfully")
            : GenericResult<TextResponse>.Success("Failed to create report");
    }
}
