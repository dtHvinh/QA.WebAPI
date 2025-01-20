using WebAPI.Response;

namespace WebAPI.Response.QuestionResponses;

public record UpdateQuestionResponse(string Message)
    : GenericResponse(Message);
