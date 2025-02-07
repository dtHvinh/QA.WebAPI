namespace WebAPI.Dto;

public record UpdateQuestionDto(int Id, string Title, string Content, List<int> Tags);
