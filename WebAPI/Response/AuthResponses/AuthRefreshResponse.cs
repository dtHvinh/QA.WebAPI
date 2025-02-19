namespace WebAPI.Response.AuthResponses;

public record AuthRefreshResponse(string AccessToken, string RefreshToken);
