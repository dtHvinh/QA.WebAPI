using WebAPI.Response.QuestionResponses;

namespace WebAPI.Response.BookmarkResponses;

public record BookmarkResponse(int Id, DateTime CreatedAt, GetQuestionResponse Question);
