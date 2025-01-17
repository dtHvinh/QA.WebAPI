using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateReportHandler(IReportRepository reportRepository, AuthenticationContext authContext)
    : ICommandHandler<CreateQuestionReportCommand, OperationResult<CreateReportResponse>>
{
    private readonly IReportRepository _reportRepository = reportRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<OperationResult<CreateReportResponse>> Handle(CreateQuestionReportCommand request, CancellationToken cancellationToken)
    {
        var report = request.Report.ToQuestionReport(_authContext.UserId);

        _reportRepository.AddQuestionReport(report);

        var op = await _reportRepository.SaveChangesAsync(cancellationToken);

        return !op.IsSuccess
            ? OperationResult<CreateReportResponse>.Failure("Failed to create report")
            : OperationResult<CreateReportResponse>.Success(
                new CreateReportResponse("Report created successfully"));
    }
}
