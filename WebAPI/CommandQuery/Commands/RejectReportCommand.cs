using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record RejectReportCommand(int ReportId)
    : ICommand<GenericResult<TextResponse>>;
