using System.Security.Claims;

namespace WebAPI.Utilities.Context;

public class AuthenticationContext(IHttpContextAccessor hca)
{
    private readonly HttpContext? _httpContext = hca.HttpContext;

    public Guid UserId => Guid.Parse(_httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
    public string Role => _httpContext?.User?.FindFirst(ClaimTypes.Role)!.Value!;
}
