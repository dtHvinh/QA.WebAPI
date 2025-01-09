namespace WebAPI.Dto;

public record AuthResponseDto(string AccessToken, string RefreshToken, string Username, string ProfilePicture);