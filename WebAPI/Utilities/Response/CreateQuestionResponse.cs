using WebAPI.Dto;

namespace WebAPI.Utilities.Response;

public record CreateQuestionResponse(Guid Id, string Title, string Content, List<TagDto> TagObjects);
