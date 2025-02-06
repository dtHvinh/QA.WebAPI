using WebAPI.Response.QuestionResponses;

namespace WebAPI.Response.BookmarkResponses;

public record BookmarkResponse(Guid Id, DateTime CreatedAt, GetQuestionResponse Question);
