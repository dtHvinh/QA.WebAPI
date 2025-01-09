namespace WebAPI.Dto;

public record RegisterDto(
    string Email, string Password, string FirstName, string LastName);
