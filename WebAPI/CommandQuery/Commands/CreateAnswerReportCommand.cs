using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateAnswerReportCommand(CreateReportDto Report) : ICommand<OperationResult<CreateReportResponse>>;
