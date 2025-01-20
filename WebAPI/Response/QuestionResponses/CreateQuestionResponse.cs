namespace WebAPI.Response.QuestionResponses;

public record CreateQuestionResponse(Guid Id, string Title, string Content, List<Guid> Tags);
