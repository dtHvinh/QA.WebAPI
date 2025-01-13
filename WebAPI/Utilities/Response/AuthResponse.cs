namespace WebAPI.Utilities.Response;

public record AuthResponse(string AccessToken, string RefreshToken, string Username, string ProfilePicture);