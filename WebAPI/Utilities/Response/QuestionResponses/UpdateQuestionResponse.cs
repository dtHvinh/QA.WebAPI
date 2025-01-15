namespace WebAPI.Utilities.Response.QuestionResponses;

public record UpdateQuestionResponse(string Message)
    : GenericResponse(Message);
