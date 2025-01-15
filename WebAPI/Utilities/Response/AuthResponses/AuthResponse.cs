namespace WebAPI.Utilities.Response.AuthResponses;

public record AuthResponse(string AccessToken, string RefreshToken, string Username, string ProfilePicture);