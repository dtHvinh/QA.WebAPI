using WebAPI.Dto;

namespace WebAPI.Utilities.Response.QuestionResponses;

public record CreateQuestionResponse(Guid Id, string Title, string Content, List<TagDto> TagObjects);
