namespace WebAPI.Dto;

public record CreateCommunityDto(string Name, string? Description, string? IconImage, bool IsPrivate);
