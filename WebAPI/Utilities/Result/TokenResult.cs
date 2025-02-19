namespace WebAPI.Utilities.Result;

public record TokenResult(bool IsSuccess, string? AccessToken, string? RefreshToken, List<string>? Errors);
