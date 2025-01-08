namespace WebAPI.Dto;

public record RegisterDto(
    string Username, string Password, string FirstName, string LastName);
