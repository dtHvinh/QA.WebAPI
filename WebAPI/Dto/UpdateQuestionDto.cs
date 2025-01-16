namespace WebAPI.Dto;

public record UpdateQuestionDto(Guid Id, string Title, string Content, List<Guid> Tags);
