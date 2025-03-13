namespace WebAPI.Response;

public record CreateReportResponse(string Message) : TextResponse(Message);
