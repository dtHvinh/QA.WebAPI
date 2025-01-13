namespace WebAPI.Dto;

public record CreateQuestionDto(string Title, string Content, List<CreateQuestionTag> Tags);

public record CreateQuestionTag(Guid Id, string Name);