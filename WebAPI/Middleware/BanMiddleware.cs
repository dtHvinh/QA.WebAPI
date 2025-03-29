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
        var id = GetId(token);
        var banTo = await _cacheService.IsBanned(id, context.RequestAborted);

        if (banTo.HasValue)
        {
            var result = ProblemResultExtensions.BadRequest("You are banned to " + banTo);

            var json = JsonSerializer.Serialize(result);
            var bytes = Encoding.UTF8.GetBytes(json);

            // TODO: document this status code
            context.Response.StatusCode = 444;
            await context.Response.BodyWriter.WriteAsync(bytes);

            await context.Response.CompleteAsync();

            return;
        }
        try
        {

            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static int GetId(string accessToken)
    {
        JwtSecurityTokenHandler handler = new();

        var securityJwt = handler.ReadJwtToken(accessToken);

        var userId = int.Parse(securityJwt.Claims.First(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value);

        return userId;
    }
}
