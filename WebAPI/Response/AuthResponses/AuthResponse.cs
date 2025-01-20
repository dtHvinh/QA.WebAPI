namespace WebAPI.Response.AuthResponses;

public record AuthResponse(string AccessToken, string RefreshToken, string Username, string ProfilePicture);