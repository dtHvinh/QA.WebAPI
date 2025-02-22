namespace WebAPI.Dto;

public record CreateCollectionDto(
    string Name,
    string? Description,
    bool IsPublic
);
