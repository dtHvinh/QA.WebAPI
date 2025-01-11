using System.Security.Claims;

namespace WebAPI.Utilities.Context;

public class AuthentcationContext(HttpContext httpContext)
{
    private readonly HttpContext _httpContext = httpContext;

    public Guid UserId => Guid.Parse(_httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
    public string Role => _httpContext.User?.FindFirst(ClaimTypes.Role)!.Value!;

}
