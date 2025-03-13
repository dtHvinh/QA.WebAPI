namespace WebAPI.Response.QuestionResponses;

public record UpdateQuestionResponse(string Message)
    : TextResponse(Message);
