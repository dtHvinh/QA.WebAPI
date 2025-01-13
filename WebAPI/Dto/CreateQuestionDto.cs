namespace WebAPI.Dto;

public record CreateQuestionDto(string Title, string Content, List<CreateQuestionTag> TagObjects);

public record CreateQuestionTag(Guid Id, string Name);