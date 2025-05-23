using WebAPI.Response.QuestionResponses;

namespace WebAPI.Response.BookmarkResponses;

public record BookmarkResponse(int Id, DateTimeOffset CreationDate, GetQuestionResponse Question);
