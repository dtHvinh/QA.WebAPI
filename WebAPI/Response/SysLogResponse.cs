namespace WebAPI.Response;

public record SysLogResponse(string Id, string Level, string Message, string UtcTimestamp);
