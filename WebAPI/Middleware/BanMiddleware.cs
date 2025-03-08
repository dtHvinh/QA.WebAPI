using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;

namespace WebAPI.Middleware;

public class BanMiddleware(RequestDelegate next, ICacheService cacheService)
{
    private readonly RequestDelegate _next = next;
    private readonly ICacheService _cacheService = cacheService;

    public async Task InvokeAsync(HttpContext context)
    {
        var auth = context.Request.Headers.Authorization.FirstOrDefault();

        if (auth is null || !auth.Skip(7).Any())
        {
            await _next(context);
            return;
        }

        var token = string.Concat(auth.Skip(7));

        var banTo = await _cacheService.IsBanned(GetId(token), context.RequestAborted);

        if (banTo.HasValue)
        {
            var result = ProblemResultExtensions.BadRequest("You are banned to " + banTo);

            var json = JsonSerializer.Serialize(result);
            var bytes = Encoding.UTF8.GetBytes(json);

            context.Response.StatusCode = 401;
            await context.Response.BodyWriter.WriteAsync(bytes);

            return;
        }

        await _next(context);
    }

    private static int GetId(string accessToken)
    {
        JwtSecurityTokenHandler handler = new();

        var securityJwt = handler.ReadJwtToken(accessToken);

        var userId = int.Parse(securityJwt.Claims.First(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value);

        return userId;
    }
}
