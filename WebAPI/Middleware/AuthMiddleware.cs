namespace WebAPI.Middleware;

public class AuthMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var auth = context.Request.Headers.Authorization;
        await _next(context);
    }
}
