namespace WebAPI.Utilities.Response;

public record CreateReportResponse(string Message) : GenericResponse(Message);
