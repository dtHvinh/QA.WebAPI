namespace WebAPI.Response.QuestionResponses;

public record CreateQuestionResponse(int Id, string Title, string Slug, string Content, List<int> Tags);
