namespace WebAPI.Dto;

public record CreateQuestionDto(string Title, string Content, List<string> Tags);
