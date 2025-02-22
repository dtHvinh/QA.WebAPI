namespace WebAPI.Dto;

public record UpdateCollectionDto(int Id, string Name, string? Description, bool IsPublic);
