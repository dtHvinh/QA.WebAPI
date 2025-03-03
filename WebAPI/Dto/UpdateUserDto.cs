namespace WebAPI.Dto;

public record UpdateUserDto(string? Username, List<LinkDto> Links);
