namespace WebAPI.Utilities.Response;

public record UpdateQuestionResponse(string Message)
    : GenericResponse(Message);
