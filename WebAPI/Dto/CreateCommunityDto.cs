namespace WebAPI.Dto;

public record CreateCommunityDto
    (string Name, string? Description, IFormFile? IconImage, bool IsPrivate);
